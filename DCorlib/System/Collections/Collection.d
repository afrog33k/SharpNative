module System.Collections.Collection;
import System.Collections.Namespace;

class Collection(T) : IList!(T) {
	
	private IList!(T) items_;
	
	this() {
		this(new List!(T));
	}
	
	this(IList!(T) list) {
		items_ = list;
	}
	
	final void add(T item) {
		insertItem(items_.count, item);
	}
	
	final void insert(int index, T item) {
		insertItem(index, item);
	}
	
	final bool remove(T item) {
		int index = items_.indexOf(item);
		if (index < 0)
			return false;
		removeItem(index);
		return true;
	}
	
	final void removeAt(int index) {
		removeItem(index);
	}
	
	final void clear() {
		clearItems();
	}
	
	final int indexOf(T item) {
		return items_.indexOf(item);
	}
	
	final bool contains(T item) {
		return items_.contains(item);
	}
	
	@property final int count() {
		return items_.count;
	}
	
	final void opIndexAssign(T value, int index) {
		setItem(index, value);
	}
	final T opIndex(int index) {
		return items_[index];
	}
	
	version (UseRanges) {
		final bool empty() {
			return items_.empty;
		}
		
		final void popFront() {
			items_.popFront();
		}
		
		final T front() {
			return items_.front;
		}
	}
	else {
		final int opApply(int delegate(ref T) action) {
			return items_.opApply(action);
		}
	}
	
	protected void insertItem(int index, T item) {
		items_.insert(index, item);
	}
	
	protected void removeItem(int index) {
		items_.removeAt(index);
	}
	
	protected void clearItems() {
		items_.clear();
	}
	
	protected void setItem(int index, T value) {
		items_[index] = value;
	}
	
	protected IList!(T) items() {
		return items_;
	}
	
}