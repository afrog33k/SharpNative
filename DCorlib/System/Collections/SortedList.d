module System.Collections.SortedList;
import System.Collections.Namespace;
//import System.Collections.IComparer_T;

class SortedList(K, V) {
	
	private const int DEFAULT_CAPACITY = 4;
	
	private IComparer!(K) comparer_;
	private K[] keys_;
	private V[] values_;
	private int size_;
	
	this() {
		comparer_ = Comparer!(K).instance;
	}
	
	this(int capacity) {
		keys_.length = capacity;
		values_.length = capacity;
		comparer_ = Comparer!(K).instance;
	}
	
	final void add(K key, V value) {
		int index = binarySearch!(K)(keys_, 0, size_, key, &comparer_.compare);
		insert(~index, key, value);
	}
	
	final bool remove(K key) {
		int index = indexOfKey(key);
		if (index >= 0)
			removeAt(index);
		return index >= 0;
	}
	
	final void removeAt(int index) {
		size_--;
		if (index < size_) {
			.copy(keys_, index + 1, keys_, index, size_ - index);
			.copy(values_, index + 1, values_, index, size_ - index);
		}
		keys_[size_] = K.init;
		values_[size_] = V.init;
	}
	
	final void clear() {
		.clear(keys_, 0, size_);
		.clear(values_, 0, size_);
		size_ = 0;
	}
	
	final int indexOfKey(K key) {
		int index = binarySearch!(K)(keys_, 0, size_, key, &comparer_.compare);
		if (index < 0)
			return -1;
		return index;
	}
	
	final int indexOfValue(V value) {
		foreach (i, v; values_) {
			if (equalityComparisonImpl(v, value))
				return cast(int)i;
		}
		return -1;
	}
	
	final bool containsKey(K key) {
		return indexOfKey(key) >= 0;
	}
	
	final bool containsValue(V value) {
		return indexOfValue(value) >= 0;
	}
	
	final int count() {
		return size_;
	}
	
	final void capacity(int value) {
		if (value != keys_.length) {
			keys_.length = value;
			values_.length = value;
		}
	}
	final int capacity() {
		return cast(int)keys_.length;
	}
	
	final K[] keys() {
		return keys_.dup;
	}
	
	final V[] values() {
		return values_.dup;
	}
	
	final V opIndex(K key) {
		int index = indexOfKey(key);
		if (index >= 0)
			return values_[index];
		return V.init;
	}
	
	private void insert(int index, K key, V value) {
		if (size_ == keys_.length)
			ensureCapacity(size_ + 1);
		
		if (index < size_) {
			.copy(keys_, index, keys_, index + 1, size_ - index);
			.copy(values_, index, values_, index + 1, size_ - index);
		}
		
		keys_[index] = key;
		values_[index] = value;
		size_++;
	}
	
	private void ensureCapacity(int min) {
		int n = (keys_.length == 0) ? DEFAULT_CAPACITY : keys_.length * 2;
		if (n < min)
			n = min;
		this.capacity = n;
	}
	
}

class SortedList(K, V, U)
{
}