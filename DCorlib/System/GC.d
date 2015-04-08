module System.GC;


class GC
{
	/*
	struct GCStats
	{
    size_t poolsize;        // total size of pool
    size_t usedsize;        // bytes allocated
    size_t freeblocks;      // number of blocks marked FREE
    size_t freelistsize;    // total of memory on free lists
    size_t pageblocks;      // number of blocks marked PAGE
	}*/
	public static void Collect()
	{
		import core.memory;
		core.memory.GC.collect();
	}

	public static long GetTotalMemory(bool forceFullCollection)
	{
		/*import core.memory;
		import gc.gc;

		if(forceFullCollection)
			Collect();

		GCStats stats;
		core.memory.GC.getStats(stats);
		return stats.poolsize;*/
		return 0;
	}
	

}