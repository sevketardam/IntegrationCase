using Integration.Common;
using Integration.Backend;
using System.Collections.Concurrent;

namespace Integration.Service;

public sealed class ItemIntegrationService
{
    //This is a dependency that is normally fulfilled externally.
    private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();

    private static readonly ConcurrentDictionary<string, object> _locks = new ConcurrentDictionary<string, object>();

    // This is called externally and can be called multithreaded, in parallel.
    // More than one item with the same content should not be saved. However,
    // calling this with different contents at the same time is OK, and should
    // be allowed for performance reasons.
    public async Task<Result> SaveItem(string itemContent)
    {
        string lockKey = $"lock:item:{itemContent}";

        bool lockAcquired = MemoryLockService.AcquireLock(lockKey);

        if (!lockAcquired)
        {
            return new Result(false, $"Duplicate item received with content {itemContent}. Another process is working on it.");
        }

        try
        {
            var lockObject = _locks.GetOrAdd(itemContent, new object());

            lock (lockObject)
            {
                // Check the backend to see if the content is already saved.
                if (ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
                {
                    return new Result(false, $"Duplicate item received with content {itemContent}.");
                }

                var item = ItemIntegrationBackend.SaveItem(itemContent);

                return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
            }
        }
        finally
        {
            MemoryLockService.ReleaseLock(lockKey);
        }
    }

    public List<Item> GetAllItems()
    {
        return ItemIntegrationBackend.GetAllItems();
    }
}