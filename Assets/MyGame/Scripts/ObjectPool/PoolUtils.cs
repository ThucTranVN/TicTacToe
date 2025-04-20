using System.Collections.Generic;

public static class PoolHelper
{
	// Pool container for given type
	private static class PoolsOfType<T> where T : class
	{
		// Pool with poolName = null
		private static ObjectPool<T> defaultPool = null;

		// Other pools
		private static Dictionary<string, ObjectPool<T>> namedPools = null;

		public static ObjectPool<T> GetPool(string poolName = null)
		{
			if (poolName == null)
			{
				if (defaultPool == null)
					defaultPool = new ObjectPool<T>();

				return defaultPool;
			}
			else
			{
				ObjectPool<T> result;

				if (namedPools == null)
				{
					namedPools = new Dictionary<string, ObjectPool<T>>();

					result = new ObjectPool<T>();
					namedPools.Add(poolName, result);
				}
				else if (!namedPools.TryGetValue(poolName, out result))
				{
					result = new ObjectPool<T>();
					namedPools.Add(poolName, result);
				}

				return result;
			}
		}
	}

	// NOTE: if you don't need two or more pools of same type,
	// leave poolName as null while calling any of these functions
	// for better performance

	public static ObjectPool<T> GetPool<T>(string poolName = null) where T : class
	{
		return PoolsOfType<T>.GetPool(poolName);
	}

	public static void Push<T>(T obj, string poolName = null) where T : class
	{
		PoolsOfType<T>.GetPool(poolName).Push(obj);
	}

	public static T Pop<T>(string poolName = null) where T : class
	{
		return PoolsOfType<T>.GetPool(poolName).Pop();
	}

	// Extension method as a shorthand for Push function
	public static void Pool<T>(this T obj, string poolName = null) where T : class
	{
		PoolsOfType<T>.GetPool(poolName).Push(obj);
	}
}