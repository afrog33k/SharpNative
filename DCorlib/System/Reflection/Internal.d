module System.Reflection.Internal;
//Based on ShardTool.Reflection Copyright: © 2013 Ognjen Ivkovic
/// Provides runtime reflection data for D symbols, such as classes or modules.
/// All reflection data is generated at compile-time, and thus metadata must be specified first.
/// Once a symbol has metadata generated for it, any symbols it can access will have a lazy
/// loader set to generate metadata for those symbols.
/// For example, a method returning an instance of Foo will result in a lazy loader
/// being created to generate metadata for Foo.
/**
Example:
---
class Foo {
int _val;
this(int val) {
this._val = val;
}

@property int val() const {
return _val;
}

@property void val(int value) {
_val = value;
}

int getSquare(int input) {
return input * input;
}
}

// First, we need to generate metadata for Foo.
auto metadata = createMetadata!Foo;
// Then we can create an instance.
// Note that creating an instance returns a Variant, so the result must be converted.
Variant varInstance = metadata.createInstance(3);
Foo instance = varInstance.get!Foo;
// Can easily get any property or field through getValue.
assert(metadata.getValue("val", instance) == 3);
// Since the type may not be known at compile-time, we can also operate on the Variant directly.
// Note that in this scenario the metadata will have to be created through createMetadata prior to this.
// We created it above with createMetadata!Foo, so we're fine.
assert(metadata == varInstance.metadata);
assert(metadata.getValue("val", varInstance) == 3);
// Of course, can set values and invoke methods as well.
metadata.setValue("val", instance, 6);
assert(metadata.getValue("val", instance) == 6);
assert(metadata.invokeMethod("getSquare", instance, 4) == 16);
---
*/
/// License: <a href="http://www.boost.org/LICENSE_1_0.txt">Boost License 1.0</a>
/// Authors: Ognjen Ivkovic
/// Copyright: © 2013 Ognjen Ivkovic
//module ShardTools.Reflection;
//import ShardTools.ExceptionTools;
import std.range;
import std.variant;
import std.traits;
import std.algorithm;
import std.typetuple;
import std.conv;
import std.string;
version(logreflect) import std.stdio;
import core.stdc.string;
import core.memory;
import std.exception;
import core.stdc.stdlib;
import std.math;
//import ShardTools.Udas;
mixin(MakeException("InvalidOperationException", "The performed operation was considered invalid for the present state."));
mixin(MakeException("NotSupportedException", "The operation being performed was not supported."));
mixin(MakeException("TimeoutException", "The operation reached the maximum timeout time before being complete."));
mixin(MakeException("DuplicateKeyException", "An object with this key already exists."));
mixin(MakeException("InvalidFormatException", "The data passed in was in an invalid format."));
mixin(MakeException("KeyNotFoundException", "The specified key was not found in this collection."));

/// Returns a string to make an exception with the given name and default details, optionally including the base class.
string MakeException(string ExceptionName, string ExceptionDetails, string Base = "Exception") {
	//const char[] MakeException = 
	return
		"public class " ~ ExceptionName ~ " : " ~ Base ~ " {
		public:				
		this(string ExceptionDetails = \"" ~ ExceptionDetails ~ "\", string File = __FILE__, size_t Line = __LINE__) {
		super(ExceptionDetails, File, Line);
		}
		}";
}

/// Creates an exception with the given name that does not have a default message.
string MakeException(string ExceptionName) {
	//const char[] MakeException = 
	return
		"public class " ~ ExceptionName ~ " : Exception {
		public:				
		this(string ExceptionDetails, string File = __FILE__, size_t Line = __LINE__) {
		super(ExceptionDetails, File, Line);
		}
		}";
}

/// Provides simple user-defined attributes that are commonly required.
/// License: <a href="http://www.boost.org/LICENSE_1_0.txt">Boost License 1.0</a>
/// Authors: Ognjen Ivkovic
/// Copyright: Â© 2013 Ognjen Ivkovic
//module ShardTools.Udas;

/+/// Indicates that a struct is a user-defined attribute and should not be used otherwise.
/// This attribute has no effect and is only for documentation purposes.
struct Uda { }+/

/// Provides a user defined attribute that can give a default description for a field.
struct Description {
	string value;
}

/// Provides an alternative friendly name for the field to display.
struct DisplayName {
	string value;
}

/// Indicates that this field should be ignored or not displayed to the user.
struct Ignore { 
	bool value;
}

/// Indicates that this is a required field.
struct Required {
	bool value;
}

version(Have_tested) {
	public import tested;
	public alias name = tested.name;
} else {
	/// A basic stub for the name UDA that tested uses; exists only to prevent errors if tested is unavailable.
	struct name {
		string val;
	}
}

/// Indicates the protection level of a symbol, such as private or public.
enum ProtectionLevel {
	/// No protection level exists for this particular symbol.
	/// This is usually for symbols such as parameters that do not have protection levels.
	none_ = 0,
	/// 
	private_,
	/// 
	protected_,
	/// 
	package_,
	/// 
	public_,
	/// 
	export_
}

/// Indicates whther a value refers to a field or a property.
enum DataKind {
	/// The value is a raw field.
	field,
	/// The value is a property, possibly referring to a field.
	property,
	/// The value is an enum constant.
	constant
}

/// Indicates whether an instance of TypeMetadata provides information for a struct, class, union, interface, or enum.
/// Also includes a primitive type for built in types (int, long, etc) as well as pointers and arrays.
enum TypeKind {
	/// A primitive type, such as an int or long.
	/// Arrays and pointers are also considered primitives for the purposes of this enum.
	primitive_,
	///
	struct_,
	///
	class_,
	///
	union_,
	///
	enum_,
	///
	interface_
}

/// Indicates the modifiers that are applied to a given symbol, such as scope or static.
/// This is a flags based enum, so multiple values may be set at once.
enum SymbolModifier {
	/// No currently handled modifiers are set.
	/// Note that not all modifiers are retrieved at the moment.
	none_ = 0,
	/// Indicates if the field or method is static.
	/// BUGS:
	/// 	This currently only works on methods (including properties), not fields.
	static_ = 1,
}

/// Indicates the type constructor or qualifier of a type, such as const, shared, immutable, or inout.
/// This is a flags based enum, so multiple values may be set at once.
enum TypeQualifier {
	/// A mutable and not shared type.
	/// The actual declaration may still be marked __gshared and not thread-local however.
	none_ = 0,
	///
	const_ = 1,
	///
	immutable_ = 2,
	///
	inout_ = 4,
	///
	shared_ = 8
}

alias Variant function(ValueMetadata metadata, Variant instance) DataGetterFunction;
alias void function(ValueMetadata metadata, Variant instance, Variant value) DataSetterFunction;
alias Variant function(MethodMetadata, void*, Variant[] args) MethodInvokeFunction;
alias Variant function(TypeMetadata metadata, Variant instance, ConversionKind) TypeConversionFunction;

mixin(MakeException("ReflectionException"));

// TODO: Make all metadata immutable (except for invoke of course).
// This includes symbols.
// They already are after being loaded, it's just not typed properly.
// Actually even invoke could be immutable as it doesn't modify anything related to the metadata, just not pure.

/// Provides information about the symbol that a metadata instance represents.
/// This includes features such as name, protection, and user-defined attributes.
struct Symbol {

	this(string name, ProtectionLevel protection, Variant[] attributes, SymbolModifier modifiers) {
		this._name = name;
		this._protection = protection;
		this._attributes = attributes;
		this._modifiers = modifiers;
	}

	/// Returns the name of this symbol.
	/// In certain cases, such as anonymous structs or unnamed parameters, this may be null.
	@property string name() @safe const pure nothrow {
		return _name;
	}

	/// Returns the protection level for this symbol, such as public or private.
	/// Some symbols may not have a protection level, such as modules and parameters.
	/// In this case the value would be none.
	@property ProtectionLevel protection() @safe const pure nothrow {
		return _protection;
	}

	/// Returns the modifiers that apply to this symbol.
	@property SymbolModifier modifiers() @safe const pure nothrow {
		return _modifiers;
	}

	mixin(getIsFlagsMixin!(SymbolModifier, "modifiers"));

	/// Returns a duplicate of the array containing any attributes that apply to this symbol.
	@property Variant[] attributes()  {
		// TODO: Don't duplicate, use a readonly range instead.
		return _attributes.dup;
	}

	/// Indicates if this symbol has one or more attributes of the given type.
	/// Note that this must be the exact type of the attribute.
	bool hasAttribute(TypeInfo type) {
		foreach(ref attrib; _attributes) {
			if(attrib.type == type)
				return true;
		}
		return false;
	}


	/// Returns the last specified attribute that matches type T exactly.
	/// If no attribute is found matching that type, defaultValue is evaluated and returned.
	/// For convenience, if the attribute contains only a single field, defaultValue may be that field.
	/// In this case, the value of the field will be returned instead.
	U findAttribute(T, U)(lazy U defaultValue)
		if(is(T == U) || (__traits(compiles, T.tupleof) && T.tupleof.length == 1 && is(U == typeof(T.tupleof[0])))) {

			foreach(ref attrib; retro(_attributes)) {
				if(attrib.type == typeid(T)) {
					static if(is(T == U))
						return attrib.get!T;
					else
						return attrib.get!T.tupleof[0];
				}
			}
			return defaultValue();
		}

	/// ditto
	T findAttribute(T)(lazy T defaultValue = T.init) {
		return findAttribute!(T, T)(defaultValue());
	}

	private string _name;
	private ProtectionLevel _protection;
	private Variant[] _attributes;
	SymbolModifier _modifiers;
}

/// Provides information about a module and the symbols it contains.
struct ModuleMetadata {

	this(string name, string packageName, SymbolContainer children) {
		this._name = name;
		this._packageName = packageName;
		this._children = children;
	}

	/// Gets the name of this module, excluding the package.
	@property string name() const pure nothrow {
		return _name;
	}

	/// Gets the name of this package. This can be null if this module has no package.
	@property string packageName() const pure nothrow {
		return _packageName;
	}

	/// Returns the qualified name of this module (that is, the package name combined with the module name).
	@property string qualifiedName() const pure nothrow {
		return packageName.length == 0 ? name : (packageName ~ "." ~ name);
	}

	/// Gets the symbols that this module contains.
	@property SymbolContainer children() {
		return _children;
	}

	string toString() const pure nothrow {
		return qualifiedName;
	}

private:
	string _name;
	string _packageName;
	SymbolContainer _children;
	// TODO: Find a way to get ModuleInfo and store that, and store imported modules that way.
}

/// Stores information about a type (class, union, struct, enum, interface).
struct TypeMetadata {

	this(Symbol symbol, size_t instanceSize, TypeInfo type, TypeKind kind, TypeQualifier qualifiers, SymbolContainer children,
	     TypeInfo base, TypeInfo parent, TypeInfo[] interfaces, TypeConversionFunction converter) {

			 this._symbol = symbol;
			 this._base = base;
			 this._interfaces = interfaces;
			 this._children = children;
			 this._kind = kind;
			 this._type = type;
			 this._instanceSize = instanceSize;
			 this._parent = parent;
			 this._converter = converter;
			 this._qualifiers = qualifiers;
		 }

	alias symbol this;

	/// Converts the given variant into a variant containing an instance of this type.
	/// If castFrom is called, a cast is attempted using $(D Variant.get).
	/// Otherwise, a conversion is attempted using $(D Variant.coerce).
	Variant castFrom(T)(T other) {
		Variant v = other;
		return _converter(this, v, ConversionKind.cast_);
	}

	/// ditto
	Variant coerceFrom(T)(T other) {
		Variant v = other;
		return _converter(this, v, ConversionKind.coerce_);
	}

	/// Gets the symbol representing the type this metadata refers to.
	@property Symbol symbol() @safe pure nothrow {
		return _symbol;
	}

	@property TypeQualifier qualifiers() @safe const pure nothrow {
		return _qualifiers;
	}

	/// Gets the size of a single instance of this type.
	/// This would be the class instance size for classes, 0 for interfaces, and sizeof for everything else.
	@property size_t instanceSize() const pure nothrow {
		return _instanceSize;
	}

	/// Gets the underlying TypeInfo for this aggregate.
	@property const(TypeInfo) type() const pure nothrow {
		return _type;
	}

	/// Indicates what type this metadata applies to; either a struct, class, enum, interface, or union.
	@property TypeKind kind() const pure nothrow {
		return _kind;
	}

	/// Returns the base class for this type.
	/// If this type has no base type (Object) or can not have a base type (structs / unions / interfaces), null is returned.
	/// Enums will return the default int base if no base type is specified, otherwise they will return that base type.
	/// Classes will include Object as a base type.
	@property const(TypeInfo) base() const pure nothrow {
		return _base;
	}

	/// Returns all of the interfaces that this type directly or indirectly implements.
	/// At the moment this only applies to classes and interfaces.
	@property const(TypeInfo[]) interfaces() const pure nothrow {
		return _interfaces;
	}

	/// If this type is a nested type, returns the parent of the type.
	/// Otherwise, returns null.
	@property const(TypeInfo) parent() const pure nothrow {
		return _parent;
	}

	/// Gets the child symbols that this type contains.
	@property SymbolContainer children() {
		return _children;
	}

	string toString() {
		return symbol.protection.text[0..$-1] ~ " " ~ kind.text[0..$-1] ~ " " ~ symbol.name;
	}

	bool opEquals(in TypeMetadata other) const {
		return _type == other._type && _base == other._base && _interfaces == other._interfaces && _parent == other._parent && _instanceSize == other._instanceSize 
			&& _children == other._children && _kind == other._kind && _symbol == other._symbol && _converter == other._converter && _qualifiers == other._qualifiers;
	}

private:
	TypeInfo _base;
	TypeInfo[] _interfaces;
	TypeInfo _type;
	TypeInfo _parent;
	size_t _instanceSize;
	SymbolContainer _children;
	TypeKind _kind;
	Symbol _symbol;
	TypeConversionFunction _converter;
	TypeQualifier _qualifiers;
}

/// Provides a means of storing symbols within a type or module.
struct SymbolContainer {

	// TODO: Don't duplicate; get a real non-transitive read-only return type instead.
	// Or better yet, just make it all immutable like it should be in the first place.

	/// Returns all of the types that this symbol contains.
	@property const(TypeInfo[]) types() const {
		return _types;
	}

	/// Returns a copy of all of the methods that this symbol contains.
	/// This does not include properties, as those are considered values.
	@property MethodMetadata[] methods() {
		return _methods.dup;
	}

	/// Returns a copy of all of the fields and properties that this symbol contains.
	@property ValueMetadata[] values() {
		return _values.dup;
	}

private:
	TypeInfo[] _types;
	MethodMetadata[] _methods;
	ValueMetadata[] _values;
}

/// Represents a single parameter in a method call.
struct ParameterMetadata {

	this(TypeInfo type, string name, bool hasDefaultValue, Variant defaultValue, ParameterStorageClass modifiers) {
		this._type = type;
		this._name = name;
		this._hasDefaultValue = hasDefaultValue;
		this._defaultValue = defaultValue;
		this._modifiers = modifiers;
	}

	/// Gets the type of this parameter.
	@property const(TypeInfo) type() const pure nothrow {
		return _type;
	}
	/// Gets the name of this parameter, or null if this parameter is anonymous.
	@property string name() const pure nothrow {
		return _name;
	}

	/// Indicates if this parameter has a default value available.
	@property bool hasDefaultValue() const pure nothrow {
		return _hasDefaultValue;
	}

	/// Gets the default value of this parameter.
	/// The result is undefined if $(D, hasDefaultValue) is false.
	@property const(Variant) defaultValue() const   {
		return _defaultValue;
	}

	/// Gets the attributes for this parameter.
	@property ParameterStorageClass modifiers() const pure nothrow {
		return _modifiers;
	}

	string toString() const {
		string result = modifiers == ParameterStorageClass.none ? "" : (modifiers.text ~ " ");
		result ~= type.text ~ (name == null ? "" : (" " ~ name));
		if(hasDefaultValue)
			result ~= " = " ~ defaultValue.text;
		return result;
	}

private:
	TypeInfo _type;
	Variant _defaultValue;
	ParameterStorageClass _modifiers;
	bool _hasDefaultValue;
	string _name;
}

/// Provides metadata for a method (including a constructor), providing the return type, parameters, and invocation data.
struct MethodMetadata {

	this(Symbol symbol, MethodInvokeFunction invoker, ParameterMetadata[] parameters, TypeInfo returnType, size_t vtblSlot) {
		this._symbol = symbol;
		this._invoker = invoker;
		this._parameters = parameters;
		this._returnType = returnType;
		this._vtblSlot = vtblSlot;
	}

	alias symbol this;

	/// Gets the index of this function within the vtbl of the type containing this method.
	/// For an interface this is the index within the interface, which the type has a pointer to.
	/// This value is 0 for for non-virtual methods (including final methods overriding non-final children).
	@property size_t vtblSlot() const pure nothrow {
		return _vtblSlot;
	}

	/// Gets the symbol of the method this instance provides metadata for.
	@property Symbol symbol() @safe pure nothrow {
		return _symbol;
	}

	/// Gets the return type of this method.
	@property const(TypeInfo) returnType() const pure nothrow {
		return _returnType;
	}

	/// Gets the parameters that are used as arguments to this method.
	/// At the moment varadic arguments are not supported.
	@property const(ParameterMetadata[]) parameters() const pure nothrow {
		return _parameters;
	}

	/// Finds the parameter within this method that has the given name, or init if not found.
	ParameterMetadata findParameter(string name) {
		ptrdiff_t index = _parameters.countUntil!"a.name == b"(name);
		return index == -1 ? ParameterMetadata.init : _parameters[index];
	}

	/// Invokes this method with the given arguments.
	/// If the method can not be invoked with the given arguments, an exception is thrown.
	/// In the case of static methods, instance may be null; otherwise instance must not be null.
	/// For static methods the value of instance is ignored.
	Variant invoke(InstanceType, T...)(InstanceType instance, T arguments) {
		// TODO: Warn if impure, non-static, and struct not passed in by ref.
		// Maybe with debugreflect.
		// Just need to support things like isStatic first.
		return invokeInternal(&instance, arguments);
	}

	/// ditto
	Variant invoke(InstanceType, T...)(ref InstanceType instance, T arguments) if(is(InstanceType == struct)) {
		return invokeInternal(&instance, arguments);
	}

	private Variant invokeInternal(InstanceType, T...)(InstanceType* instancePtr, T arguments) {
		// We're allowed to pass parameters as a Variant[].
		static if(T.length == 1 && is(T[0] == Variant[])) {
			Variant[] args = arguments[0];
		} else {
			// TODO: Can we stack allocate this?
			Variant[] args = new Variant[arguments.length];
			foreach(i, arg; arguments)
				args[i] = arg;
		}
		// If it's a class, the context pointer should be the reference.
		// Otherwise if it's a struct, a pointer to the struct.
		// If they just pass in the pointer directly, we'll use that though it's unsafe.
		static if(isPointer!InstanceType) {
			void* contextPtr = cast(void*)*instancePtr;
		} else static if(is(InstanceType == struct)) {
			static if(isVariant!InstanceType) {
				enum storeIndex = fieldIndex!(InstanceType, "store");
				void* contextPtr;
				ClassInfo ci = cast(ClassInfo)instancePtr.type;
				if(cast(ClassInfo)instancePtr.type)
					contextPtr = cast(void*)instancePtr.coerce!Object;
				else if(cast(TypeInfo_Interface)instancePtr.type) {
					// We need a way to get the pointer to the value stored within the Variant.
					// We can't just coerce to void* or Object, and we don't know the Interface type.
					// So we do the only reasonable thing and access the private store backing and hack around it.
					auto contents = instancePtr.tupleof[storeIndex][];
					void* interPtr = *cast(void**)contents.ptr;
					Interface* inter = **cast(Interface***)interPtr;
					contextPtr = interPtr - inter.offset;
				} else {
					if(instancePtr.type.tsize > instancePtr.size) {
						// For a large struct, Variant will store a pointer inside it's array.
						auto contentsPtr = &instancePtr.tupleof[storeIndex];
						contextPtr = *(cast(void**)contentsPtr);
					} else {
						auto contentsPtr = &instancePtr.tupleof[storeIndex];
						contextPtr = cast(void*)contentsPtr;
					}
				}
			} else {
				void* contextPtr = cast(void*)instancePtr;
			}
		} else static if(is(InstanceType == interface)) {
			// If it's an interface, casting to void* is offset by the instance's offset value.
			void* contextPtr = cast(void*)cast(Object)*instancePtr;
		} else {
			void* contextPtr = cast(void*)(*instancePtr);
		}
		return _invoker(this, contextPtr, args);
	}

	string toString() {
		string result = symbol.protection.text[0..$-1] ~ " " ~ returnType.text ~ " " ~ symbol.name ~ "(";
		foreach(ref param; _parameters) {
			result ~= param.text ~ ", ";
		}
		if(_parameters.length > 0)
			result = result[0..$-2];
		result ~= ")";
		return result;
	}

	bool opEquals(in MethodMetadata other) const {
		return _invoker == other._invoker && _parameters == other._parameters && _returnType == other._returnType && _symbol == other._symbol && _vtblSlot == other._vtblSlot && _functionPtr == other._functionPtr;
	}

private:
	MethodInvokeFunction _invoker;
	ParameterMetadata[] _parameters;
	TypeInfo _returnType;
	Symbol _symbol;
	size_t _vtblSlot;
	void* _functionPtr;
}

/// Provides metadata for fields or properties.
/// Even if a property has more than one setter, it will still be represented by a single instance of ValueMetadata.
/// The setter used will then be dependent on the type passed in to setValue.
struct ValueMetadata {

	this(T)(Symbol symbol, DataKind kind, TypeInfo type, DataGetterFunction getter, DataSetterFunction setter, T backingData)
		if(is(T == FieldValueMetadata) || is(T == PropertyValueMetadata)) {
			this._symbol = symbol;
			this._type = type;
			this._getter = getter;
			this._setter = setter;
			this._kind = kind;
			static if(is(T == FieldValueMetadata))
				this._fieldData = backingData;
			else
				this._propertyData = backingData;
		}

	alias symbol this;

	/// Gets the symbol that this instance provides metadata for.
	@property Symbol symbol() @safe pure nothrow {
		return _symbol;
	}

	/// Indicates whether this value is a field or a property.
	@property DataKind kind() const pure nothrow {
		return _kind;
	}

	/// Gets information about the field that this value references.
	/// If kind is not set to field or constant, a ReflectionException is thrown.
	@property FieldValueMetadata fieldData() {
		if(kind != DataKind.constant && kind != DataKind.field)
			throw new ReflectionException("Unable to get the field data for a value that is not a field or constant.");
		return _fieldData;
	}

	/// Gets information about the property that this value references.
	/// This includes both the getter method and setter methods for the value.
	/// Each method contained by this value contains the appropriate symbol.
	/// This allows you to get method-specific attributes or other such data.
	@property PropertyValueMetadata propertyData() {
		if(kind != DataKind.property)
			throw new ReflectionException("Unable to get the property data for a value that is not a property.");
		return _propertyData;
	}

	/// Gets the type of the value.
	@property const(TypeInfo) type() const pure nothrow {
		return _type;
	}

	/// Indicates whether this value can be changed dynamically.
	/// This would return true for fields, readable properties, and user defined attributes.
	@property bool canGet() const pure nothrow {
		return _getter !is null;
	}

	/// Indicates whether this value can be changed dynamically.
	/// This would return true for fields and properties with setters, and false for read-only properties and user defined attributes.
	/// For enum values, this is always false.
	@property bool canSet() const pure nothrow {
		return _getter !is null;
	}

	/// Returns the value that the given instance has for this member.
	/// If the value is static or an enum value, instance is ignored and may be null.
	Variant getValue(InstanceType)(InstanceType instance) {
		if(!canGet)
			throw new NotSupportedException("Attempted to get a value on a member that did not support it.");
		Variant result = _getter(this, Variant(instance));
		return result;
	}

	/// Sets this member on the specified instance to the given value.
	/// If the value is static, instance should be init.
	void setValue(InstanceType, ValueType)(InstanceType instance, ValueType value) {
		// TODO: Need to so support checking for static.
		//if(is(InstanceType == struct) && !isStatic)
		//	throw new InvalidOperationException("A struct was passed by value to setValue to a non-static method, causing the operation to have no effect.");
		enforceCanSet(instance);
		_setter(this, wrapVariantPointer(instance), Variant(value));
	}

	/// ditto
	void setValue(InstanceType, ValueType)(ref InstanceType instance, ValueType value) if(is(InstanceType == struct)) {
		enforceCanSet(instance);
		_setter(this, wrapVariantPointer(instance), Variant(value));
	}

	private void enforceCanSet(InstanceType)(ref InstanceType instance) {
		if(!canSet)
			throw new NotSupportedException("Attempted to set a value on a member that did not support it.");
		static if(isVariant!InstanceType) {
			// This isn't really possible.
			// While yes, we could make it work in this some cases (adjusting values on a Variant),
			// this would lead to things like getValue(instance).setValue("bar", 3) and expecting instance.bar to be changed.
			// In reality, a copy of instance.bar would be changed instead.
			// TODO: This is probably feasible by just using peek.
			if(cast(TypeInfo_Struct)instance.type)
				throw new NotSupportedException("Unable to set a value on a struct contained by a Variant as it would operate on a copy.");
		}
	}

	string toString() {
		string result = (kind == DataKind.constant ? "enum" : symbol.protection.text[0..$-1])
			~ " " ~ type.text ~ " " ~ symbol.name ~ (kind == DataKind.property ? "()" : "");
		return result;
	}

	bool opEquals(in ValueMetadata other) const {
		return _type == other._type && _getter == other._getter && _setter == other._setter && _kind == other._kind && _symbol == other._symbol
			&& (_kind == DataKind.property ? _propertyData == other._propertyData : _fieldData == other._fieldData);
	}

private:
	TypeInfo _type;
	DataGetterFunction _getter;
	DataSetterFunction _setter;
	DataKind _kind;
	Symbol _symbol;
	union {
		FieldValueMetadata _fieldData;
		PropertyValueMetadata _propertyData;
	}
}

/// Provides backing information for a field referenced by a ValueMetadata instance.
struct FieldValueMetadata {

	this(size_t index, size_t offset, TypeInfo declaringType) {
		this._index = index;
		this._offset = offset;
		this._declaringType = declaringType;
	}

	/// Gets the index of the field within the type containing it.
	/// Note that it is possible for two fields to have the same index in the case
	/// where a class derived from another class that implements the field.
	/// The index is guaranteed to be unique on type however.
	@property size_t index() const {
		return _index;
	}

	/// Gets the offset, in bytes, of the location of this field within the type.
	/// The D ABI specifies that this will be the same for classes deriving from type.
	@property size_t offset() const {
		return _offset;
	}

	/// Gets the type that declares this field.
	@property TypeInfo declaringType() {
		return _declaringType;
	}

private:
	size_t _index;
	size_t _offset;
	TypeInfo _declaringType;
}

/// Provides backing information for a property referenced by a ValueMetadata instance.
/// This includes the property's getter method and all of it's setter methods.
struct PropertyValueMetadata {

	this(MethodMetadata getter, MethodMetadata[] setters) {
		this._getter = getter;
		this._setters = setters;
	}

	/// Returns the method that's used to get the value of this property.
	/// A getter is defined as a method marked with @property that has zero parameters.
	@property MethodMetadata getter() {
		return _getter;
	}

	/// Returns a duplicate of the array that contains the setter methods for this property.
	/// A setter is defined as a method marked with @property that contains parameters.
	@property MethodMetadata[] setters() {
		// TODO: Again, a readonly array return-type that doesn't make each item const.
		return _setters.dup;
	}

	private MethodMetadata _getter;
	private MethodMetadata[] _setters;
}

/+/// Provides a dynamic type that utilizes reflection metadata to forward all methods
/// to the underlying type. While dynamic will work for D types using the $(D metadata) method,
/// it's real use shines with metadata generated from an external source such as a JSON parser.
struct Dynamic {
Variant instance;
TypeMetadata metadata;
}+/

/// Returns metadata for the given object, if it's registered.
/// If it's not registered, it will attempt to be generated if possible.
/// Otherwise, TypeMetadata.init is returned.
/// If a variant is passed in, metadata for the type it contains will be returned instead.
/// If a variant is passed in that refers to a type that does not have metadata, init will be returned.
/// If T refers to a class and instance is a class derived from T for which reflection information
/// is not available, reflection data for type T will be returned instead.
TypeMetadata metadata(T)(T instance) {
	TypeInfo typeInfo;
	static if(isVariant!(Unqual!T)) {
		typeInfo = cast()instance.type;
		if(auto ci = cast(ClassInfo)typeInfo) {
			// If the Variant is storing a class, we want to still be able to handle a class
			// derived from the one statically stored. So we get the value of the actual
			// type stored within it by getting it's TypeInfo, not the one Variant tells us.
			typeInfo = typeid(instance.coerce!Object);
		}
	} else static if(is(Unqual!T : TypeInfo)) {
		typeInfo = cast()instance;
	} else {
		if(instance is null)
			typeInfo = typeid(T);
		else {
			static if(is(T == interface))
				typeInfo = (cast(Object)instance).classinfo;
			else
				typeInfo = typeid(instance);
		}
		version(logreflect) writeln("Got type info for ", typeInfo, " from ", cast()instance);
	}
	// TODO: If the type is Dynamic, return that instead.
	// TODO: Figure out a way to lock this; can't just use typeInfo because synchronized(typeid(int)) segfaults.
	synchronized(typeid(TypeMetadata)) {
		TypeMetadata existing = getStoredExisting(typeInfo, false);
		if(existing != TypeMetadata.init)
			return existing;
		static if(!isVariant!T && !is(T : TypeInfo)) {
			version(logreflect) writeln("No metadata for " ~ typeInfo.text ~ "; creating and returning metadata for " ~ typeid(T).text ~ ".");
			return createMetadata!T();
		} else {
			static if(isVariant!T) {
				version(logreflect) writeln("Got variant of unknown type " ~ instance.type.text ~ "; returning init.");
			} else {
				version(logreflect) writeln("Got TypeInfo of unknown type " ~ typeInfo.text ~ "; returning init.");
			}
			return TypeMetadata.init;
		}
	}
}

/// Generates reflection data for the given class, struct, interface, primitive, union, or enum.
/// Types that are referred to directly by the given type will be lazily loaded upon being accessed.
TypeMetadata createMetadata(Type)() {
	// TODO: Decide how to handle this synchronization.
	// typeid(T) does make sense sort of, but could lead to nasty deadlocks and is a not ideal choice.
	// Yet we want a way to lock each individual type for when we generate metadata for it...
	// And rather not have to create our own Mutex each time, and worry about storing those etc.
	// Perhaps we can take advantage of knowing we're a template, and use a static variable.
	// We could also just not bother and risk race conditions generating reflection data multiple times.
	// It probably wouldn't be a big deal since everything is structs and the AA value would be replaced.
	// Synchronizing typeid(TypeMetadata) is definitely a bad idea; need a RWMutex or Spinlock for that for sure.

	// For now, I doubt the concurrency is going to be too important so just lock a single thing (RWMutex it).
	//synchronized(typeid(T)) {
	// Below pragma could be useful in situations where you want to know exactly what's being compiled in.
	alias T = Type;
	version(debugreflect) pragma(msg, "Compiling metadata for " ~ T.stringof);
	synchronized(typeid(TypeMetadata)) {
		TypeMetadata existing = getStoredExisting(typeid(T), true);
		if(existing != TypeMetadata.init)
			return existing;
		//static if(__traits(compiles, getType!T)) {
		auto result = getType!T;
		//synchronized(typeid(TypeMetadata)) {
		_typeData[typeid(T)] = StoredTypeMetadata(result);
		//}
		return result;
		/+} else {
		pragma(msg, "Warning: Internal Error - Failed to create metadata for " ~ T.stringof ~ " as getType!T failed to compile.");
		return TypeMetadata.init;
		}+/
	}
}

private TypeMetadata getType(T)() if(is(T == Symbol) || is(T == TypeMetadata)) {
	// Can't generate metadata for types from this module.
	pragma(msg, "Warning: Skipping metadata for ", T.stringof, " due to likely linker errors.");
	return TypeMetadata.init;
}

private TypeMetadata getType(T)() if(!is(T == Symbol) && !is(T == TypeMetadata)) {
	Symbol symbol = getSymbol!T;
	static if(isPrimitive!T && !is(T == enum)) {
		SymbolContainer symbols = SymbolContainer.init;
		TypeInfo parent = null;
	} else {
		SymbolContainer symbols = getSymbols!T;
		static if(__traits(compiles, isAggregateType!(__traits(parent, T)))) {
			static if(isAggregateType!(__traits(parent, T)))
				TypeInfo parent = registerLazyLoader!(__traits(parent, T));
			else
				TypeInfo parent = null;
		} else
			TypeInfo parent = null;
	}
	TypeKind kind = getTypeKind!T;
	TypeInfo type = typeid(T);
	TypeInfo base = getBase!T;
	size_t instanceSize = getSize!T;
	TypeInfo[] interfaces = getInterfaces!T;
	// TODO: Need to make this not be Unqual.
	// The problem is that we have a Variant passed in.
	// So when we get inout, we don't know what the type we want is.
	TypeConversionFunction converter = &convertTo!(Unqual!T);
	TypeQualifier qualifiers;
	static if(is(T == immutable))
		qualifiers = TypeQualifier.immutable_;
	else static if(is(T == inout))
		qualifiers = TypeQualifier.inout_;
	else static if(is(T == const))
		qualifiers = TypeQualifier.const_;
	static if(is(T == shared))
		qualifiers |= TypeQualifier.shared_;
	TypeMetadata result = TypeMetadata(symbol, instanceSize, type, kind, qualifiers, symbols, base, parent, interfaces, converter);
	return result;
}
/// Register a loader that can get type metadata for this instance upon demand.
/// Returns the TypeInfo for the type that was registered.
TypeInfo registerLazyLoader(T)() {
	// TODO: Better locking, such as RWMutex and not locking typeid(TypeMetadata).
	synchronized(typeid(TypeMetadata)) {
		if(typeid(T) !in _typeData) {
			version(logreflect) writeln("Generating lazy loader for ", T.stringof, ".");
			_typeData[typeid(T)] = StoredTypeMetadata(&createMetadata!T);
		}
	}
	return typeid(T);
}

/// Finds the type with the given name on the specified metadata.
/// If no value is found, init is returned.
TypeMetadata findType(T)(T instance, string name) {
	foreach(ref type; instance.children._types) {
		// HACK: We don't know the actual name of the type as TypeInfo does not store the relative name.
		// So instead we just take the absolute name that .text gives us and strip past the last dot.
		string typeName = type.text;
		size_t lastDot = typeName.retro.countUntil('.');
		if(lastDot != -1)
			typeName = typeName[$-lastDot..$];
		if(typeName == name)
			return type.metadata;
	}
	return TypeMetadata.init;
}

/// Returns the value on the given type that has the specified name.
/// This name is guaranteed to be unique on that type.
/// If no value is found, init is returned.
ValueMetadata findValue(T)(T instance, string name) {
	foreach(ref val; instance.children._values) {
		if(val.name == name)
			return val;
	}
	return ValueMetadata.init;
}

/// Returns the first method with the specified name on the given instance metadata.
/// The instance must be an instance of either ModuleMetadata or TypeMetadata.
/// Only methods that can be invoked with the paramTypes are returned (with an empty array for no arguments).
/// If no method with that name and set of arguments exists, init is returned.
/// Bugs:
/// 	At the moment only exact matches and matches with conversion to const are supported.
/// 	Implicit conversions and variadic arguments are not supported.
MethodMetadata findMethod(T)(T metadata, string methodName, TypeInfo[] paramTypes...) if(is(T == TypeMetadata) || is(T == ModuleMetadata)) {
	return findMethodInternal(metadata.children._methods, methodName, paramTypes);
}

private MethodMetadata findMethodInternal(MethodMetadata[] methods, string methodName, TypeInfo[] paramTypes...) {
	// TODO: Optimizing would be nice, but not hugely important since the caller can store the result.
	// TODO: Variable length args.
	// In accordance with http://dlang.org/function.html#function-overloading, overloads are handled in the following ways (though opposite index). 
	// Index -1 is an exact match and not included because it would be returned immediately.
	// Index 0 is a match with implicit conversions.
	// Index 1 is a match with conversion to const.
	// If vararg then we need only check the specified parameters. If there are more, we can return it anyways.
	// Except that's not supported because it starts getting quite complicated to invoke...
	MethodMetadata bestMatch;
	ImplicitConversionType bestMatchType = ImplicitConversionType.none_;
	foreach(ref data; methods) {
		// TODO: Supporting implicit conversions will be harder; will likely need metadata support.
		//bool hasImplicitConversion = false;
		ImplicitConversionType worstConversion = ImplicitConversionType.exact_;
		if(data.name == methodName) {
			if(data.parameters.length != paramTypes.length)
				continue;
			bool falseParam = false;
			foreach(i, param; data.parameters) {
				auto conversionType = getImplicitConversionType(paramTypes[i], param.type);
				worstConversion = max(worstConversion, conversionType);
				if(worstConversion == ImplicitConversionType.none_)
					break;
			}
			if(worstConversion == ImplicitConversionType.exact_)
				return data;
			// Don't overwrite an implicit mismatch with an implicit + const mismatch.
			if(worstConversion < bestMatchType) {
				bestMatchType = worstConversion;
				bestMatch = data;
			}
		}
	}
	version(logreflect) {
		if(bestMatchType == ImplicitConversionType.none_)
			writeln("Did not find method with overloads of ", paramTypes, ".");
	}
	return bestMatch;
}

/// Finds the first method with the given type params, as subject to $(D, findMethod).
/// Afterwards invokes the method.
/// This is merely a shortcut to prevent having to pass in methods multiple times.
/// If no method was found that can be invoked with the given exact arguments, a ReflectionException is thrown.
Variant invokeMethod(T, InstanceType, ArgTypes...)(T metadata, string methodName, InstanceType instance, ArgTypes args) {
	TypeInfo[] paramTypes = templateArgsToTypeInfo!ArgTypes;
	MethodMetadata method = findMethod(metadata, methodName, paramTypes);
	if(method == MethodMetadata.init)
		throw new ReflectionException("Unable to find a method named " ~ methodName ~ " within " ~ metadata.name ~ " that can be invoked with these arguments.");
	return method.invoke(instance, args);
}

/// A shortcut to get or set the first value with the given name.
/// If no value exists with the given name on the specified metadata, a ReflectionException is thrown.
Variant getValue(T, InstanceType)(T metadata, string valueName, InstanceType instance) {
	ValueMetadata value = metadata.findValue(valueName);
	if(value == ValueMetadata.init)
		throw new ReflectionException("Unable to find a value named " ~ valueName ~ " within " ~ metadata.name ~ ".");
	return value.getValue(instance);
}

/// ditto
void setValue(T, InstanceType, ValueType)(T metadata, string valueName, InstanceType instance, ValueType value) {
	ValueMetadata valueData = metadata.findValue(valueName);
	if(valueData == ValueMetadata.init)
		throw new ReflectionException("Unable to find a value named " ~ valueName ~ " within " ~ metadata.name ~ ".");
	valueData.setValue(instance, value);
}

/// Creates a new instance of a type given metadata and arguments to pass in to the constructor.
/// Bugs:
/// 	At the moment creating an instance of a struct with arguments is not supported.
Variant createInstance(ArgTypes...)(TypeMetadata metadata, ArgTypes args) {
	if(metadata.kind != TypeKind.struct_ && metadata.kind != TypeKind.class_)
		throw new NotSupportedException("Only structs and classes may be instantiated through createInstance.");
	TypeInfo[] argTypes = templateArgsToTypeInfo!(ArgTypes);
	MethodMetadata method = findMethod(metadata, "__ctor", argTypes);
	if(method == MethodMetadata.init && ArgTypes.length > 0)
		throw new ReflectionException("No constructor found that matches the arguments passed in.");
	if(metadata.kind == TypeKind.struct_) {
		void[] bytes;
		// init() is null if all zeros.
		// TODO: Is it safe to allocate this on the stack?
		// Almost certain it is for the default constructor.
		// But not sure about for when passing in arguments.
		if(metadata.type.init().ptr !is null)
			bytes = cast(void[])metadata.type.init().dup;
		else
			bytes = new void[metadata.instanceSize];
		if(ArgTypes.length > 0) {
			// We can just invoke the method on the byte array directly.
			// The pointer will be correct after all, and saves us a coerceFrom.
			// Unfortunately, can't seem to get this working.
			// It's passing in the wrong argument into the constructor...
			// The part about using bytes.ptr seems to be working okay though.
			throw new ReflectionException("Calling createInstance for a struct using a non-default constructor is not yet supported.");
			/+Variant resultInstance = method.invoke(bytes.ptr, args);
			return resultInstance;+/
		} else {
			// If no constructor is used, use coerce to cast from void* to the type.
			Variant instance = metadata.coerceFrom(bytes);
			return instance;
		}
	} else {
		if(ArgTypes.length == 0) {
			ClassInfo ci = cast(ClassInfo)metadata.type;
			// We don't statically know what the result of ci.create is.
			// So we have to use something which does.
			// In this case, we can just use coerceFrom on the result since that function knows the static type.
			Object instance = ci.create();
			return metadata.coerceFrom(instance);
			//return Variant(instance);
		} else {
			ubyte[] data = cast(ubyte[])GC.malloc(metadata.instanceSize)[0 .. metadata.instanceSize];
			ClassInfo ci = cast(ClassInfo)metadata.type;
			enforce(data.length == ci.init.length);
			memcpy(data.ptr, ci.init[].ptr, ci.init.length);
			Object result = cast(Object)(cast(void*)data.ptr);
			auto retResult = method.invoke(result, args);
			assert(result == retResult.coerce!Object);
			return retResult;
		}
	}
}

/// Enforces that the given metadata is a real value as opposed to the init value returned when a given member does not exist.
/// If the member does not exist and thus $(D metadata) is invalid, a ReflectionException is thrown.
/// The same metadata passed instance passed in is returned.
@property auto assumed(T)(ref T metadata) {
	if(metadata == T.init)
		throw new ReflectionException("The metadata for the value was not found.");
	return metadata;
}

private enum ConversionKind {
	cast_,
		coerce_
}

private enum MethodParentKind {
	unknown_,
		interface_,
		class_
}

/// Indicates how the source type can be implicitly converted to the target type.
/// Guaranteed to be ordered from best to worst sequentially.
private enum ImplicitConversionType {
	/// An exact match, no conversion was required.
	exact_ = 0,
		/// A conversion to inout is required.
		/// In this situation, implicit_ may be set as well.
		inout_ = 1,
		/// A conversion to an implicit type is required.
		/// In this situation const_ may be set as well.
		implicit_ = 2,
		/// A conversion from non-const to const is required.
		/// In this situation implicit_ may be set as well.
		const_ = 4,
		// No implicit conversion is possible
		none_ = 8,
}

private Symbol getSymbol(Args...)() if(Args.length == 1) {
	alias T = Args[0];
	string name = getName!T;
	ProtectionLevel protection = getProtection!T;
	Variant[] attributes = getAttributes!T;
	SymbolModifier modifiers = SymbolModifier.none_;
	static if(__traits(isStaticFunction, T))
		modifiers |= SymbolModifier.static_;
	Symbol result = Symbol(name, protection, attributes, modifiers);
	return result;
}

private Variant[] getAttributes(Args...)() if(Args.length == 1) {
	alias T = Args[0];
	static if(!__traits(compiles, __traits(getAttributes, T))) {
		version(logreflect) writeln("Calling getAttributes on " ~ T.stringof ~ " does not compile. Returning null.");
		return null;
	} else {
		auto tup = __traits(getAttributes, T);
		return attributeTupleToArray(tup);
	}
}

private Variant[] attributeTupleToArray(T...)(T tup) {
	Variant[] result = new Variant[tup.length];
	foreach(index, attrib; tup) {
		registerLazyLoader!(typeof(attrib));
		result[index] = Variant(attrib);
	}
	return result;
}

private string getName(Args...)() if(Args.length == 1) {
	alias T = Args[0];
	static if(is(T[0]) && isPrimitive!T && !is(T == enum)) {
		return typeid(Unqual!T).text;
	} else static if(__traits(compiles, Unqual!T)) {
		return __traits(identifier, Unqual!T);
	} else {
		return __traits(identifier, T);
	}
}

private SymbolContainer getSymbols(alias T)() {
	SymbolContainer result;
	foreach(m; __traits(allMembers, T)) {
		// Fields we can handle non-public members for.
		// Not sure yet for methods or types. Probably could types, doubt methods.
		static if(is(T == enum)) {
			ValueMetadata value = getEnumValue!(T, m);
			result._values ~= value;
		} else static if(hasField!(T, m)) {
			ValueMetadata value = getField!(T, m);
			result._values ~= value;
		} else static if(is(typeof(__traits(getMember, T, m))) && is(typeof(__traits(getMember, T, m)))) {
			//version(logreflect) writeln("Processing member ", m, " on ", T.stringof, ".");
			alias aliasSelf!(__traits(getMember, T, m)) member;
			//pragma(msg, __traits(identifier, __traits(getMember, T, m)));
			static if(isType!member) {
				result._types ~= registerLazyLoader!member;
			} else static if(__traits(getOverloads, T, m).length > 0) {
				populateMethods!(T, m)(result);
			} else {
				version(logreflect) writeln("Skipped unsupported member " ~ T.stringof ~ "." ~ m ~ ".");
			}
		} else {
			version(logreflect) writeln("Skipped member ", m, " on ", T.stringof, " because it was not accessible.");
		}
	}
	return result;
}

private void populateMethods(T, string m)(ref SymbolContainer result) {
	MethodMetadata propertyGetter;
	MethodMetadata[] propertySetters;
	foreach(func; __traits(getOverloads, T, m)) {
		static if(isStaticConstructor!(func)) {
			version(logreflect) writeln("Skipping static ctor for " ~ T.stringof ~ ".");
		} else {
			auto method = getMethod!(func, T);
			static if(functionAttributes!func & FunctionAttribute.property) {
				// A getter is defined as a property that returns non-void and takes in zero parameters.
				if(arity!func == 0 && !is(ReturnType!func == void)) {
					// TODO: BUG: It is possible to have multiple getters.
					// For example, one const and one non-const.
					// Decide how this should be handled.
					// For the moment it's fine because we completely ignore const and such.
					// Obviously that's not a great solution going forward however.
					// Plus, the code could be different.
					//assert(propertyGetter == MethodMetadata.init);
					propertyGetter = method;
				} else {
					// Otherwise, it's a setter.
					propertySetters ~= method;
				}
			} else {
				result._methods ~= method;
			}
		}
	}
	if(propertyGetter != MethodMetadata.init || propertySetters.length > 0)
		result._values ~= getProperty!(T)(propertyGetter, propertySetters);
}

private bool hasField(T, string m)() {
	static if(fieldIndex!(T, m) != -1)
		return true;
	else static if(is(T== class) && BaseClassesTuple!T.length > 0)
		return hasField!(BaseClassesTuple!T[0], m);
	else
		return false;
}


private ValueMetadata getEnumValue(T, string m)() {
	enum dataKind = DataKind.constant;
	// Enums can't have attributes or protection.
	Symbol symbol = Symbol(m, ProtectionLevel.none_, null, SymbolModifier.none_);
	TypeInfo type = typeid(T);
	DataGetterFunction getter = &getEnumConstant!(T, m);
	DataSetterFunction setter = null;
	enum size_t index = fieldIndex!(T, m);
	enum size_t offset = index * T.sizeof;
	FieldValueMetadata fieldData = FieldValueMetadata(index, 0, typeid(T));
	return ValueMetadata(symbol, dataKind, type, getter, setter, fieldData);
}

private ValueMetadata getField(T, string m)() {
	enum dataKind = DataKind.field;
	enum index = fieldIndex!(T, m);
	static if(index == -1)
		return getField!(BaseClassesTuple!T[0], m);
	else {
		TypeInfo type = registerLazyLoader!(typeof(T.tupleof[index]));
		size_t offset = T.tupleof[index].offsetof; //__traits(getMember, T, m).offsetof;
		DataGetterFunction getter = &getFieldValue!(T, index);
		static if(!isMutable!(typeof(T.tupleof[index]))) {
			// Can't set const or immutable fields:
			DataSetterFunction setter = null;
		} else {
			DataSetterFunction setter = &setFieldValue!(T, index);
		}
		// Unfortunately have to duplicate this, couldn't get getSymbol to work with private fields.
		// Passing in the symbol causes it to try to be evaluated and has errors with static.
		alias FieldType = typeof(T.tupleof[index]);
		string name = m;
		ProtectionLevel protection = to!ProtectionLevel(__traits(getProtection, __traits(getMember, T, m)) ~ "_");
		auto unparsedAttribs = __traits(getAttributes, T.tupleof[index]);
		Variant[] attributes = attributeTupleToArray(unparsedAttribs);
		SymbolModifier modifiers = SymbolModifier.none_;
		Symbol symbol = Symbol(name, protection, attributes, modifiers);
		TypeInfo declaringType = typeid(T);
		FieldValueMetadata fieldData = FieldValueMetadata(index, offset, declaringType);
		return ValueMetadata(symbol, dataKind, type, getter, setter, fieldData);
	}
}

private ValueMetadata getProperty(T)(MethodMetadata getterMethod, MethodMetadata[] setterMethods) {
	enum kind = DataKind.property;
	TypeInfo type = getterMethod._returnType;
	string name = getterMethod.symbol._name;
	ProtectionLevel protection = getterMethod.symbol._protection;
	SymbolModifier modifiers = getterMethod.symbol.modifiers;
	// We default to the getter's name, protection, and modifiers, but no attributes.
	// Attributes should be gotten for each individual method.
	Symbol symbol = Symbol(name, protection, null, modifiers);
	PropertyValueMetadata propertyData = PropertyValueMetadata(getterMethod, setterMethods);
	DataGetterFunction getter = &getPropertyValue!(T);
	DataSetterFunction setter = &setPropertyValue!(T);
	return ValueMetadata(symbol, kind, type, getter, setter, propertyData);
}

private MethodMetadata getMethod(alias func, T)() {
	Symbol symbol = getSymbol!func;
	ParameterMetadata[] params = getParameters!(func);
	// Can't just set MPK inside static if due to ICE.
	static if(is(T == class))
		auto invoker = &(invokeMethod!(func, T, MethodParentKind.class_));
	else static if(is(T == interface))
		auto invoker = &(invokeMethod!(func, T, MethodParentKind.interface_));
	else {
		static assert(!__traits(isVirtualMethod, func), "Expected virtual method to have it's parent be either a class or interface.");
		auto invoker = &(invokeMethod!(func, T, MethodParentKind.unknown_));
	}
	TypeInfo returnType = registerLazyLoader!(ReturnType!func);
	static if(__traits(isVirtualMethod, func)) {
		size_t vtblSlot = __traits(getVirtualIndex, func);
	} else {
		size_t vtblSlot = 0;
	}

	return MethodMetadata(symbol, invoker, params, returnType, vtblSlot);
}

private ParameterMetadata[] getParameters(alias func)() {
	ParameterMetadata[] params;
	// NOTE: We have to use ParameterTypeTuple to handle functions with unnamed parameters.
	foreach(paramIndex, _paramType; ParameterTypeTuple!(func)) {
		TypeInfo paramType = registerLazyLoader!(ParameterTypeTuple!(func)[paramIndex]);
		static if(is(ParameterDefaultValueTuple!(func)[paramIndex] == void)) {
			bool hasDefaultValue = false;
			Variant defaultVal = null;
		} else {
			Variant defaultVal = Variant(ParameterDefaultValueTuple!(func)[paramIndex]);
			bool hasDefaultValue = true;
		}
		// TODO: The below will result in no parameters being named if even a single one is unnamed.
		// This is because it's ParameterIdentifierTuple that fails to compile.
		static if(__traits(compiles, ParameterIdentifierTuple!(func)[paramIndex])) {
			string paramName = ParameterIdentifierTuple!(func)[paramIndex];
		} else {
			string paramName = null;
		}
		ParameterStorageClass storageClass = cast(ParameterStorageClass)ParameterStorageClassTuple!(func)[paramIndex];
		ParameterMetadata paramData = ParameterMetadata(paramType, paramName, hasDefaultValue, defaultVal, storageClass);
		params ~= paramData;
	}
	return params;
}

private TypeInfo getBase(T)() {
	static if(is(T == class)) {
		// BaseClassesTuple seems to break on Object?
		static if(is(Unqual!T == Object) || BaseClassesTuple!(T).length == 0)
			return null;
		else
			return registerLazyLoader!(BaseClassesTuple!(T)[0]);
	} else static if(is(T == enum)) {
		return registerLazyLoader!(OriginalType!T);
	} else
		return null;
}

private TypeKind getTypeKind(T)() {
	static if(is(T == class))
		return TypeKind.class_;
	else static if(is(T == struct))
		return TypeKind.struct_;
	else static if(is(T == union))
		return TypeKind.union_;
	else static if(is(T == enum))
		return TypeKind.enum_;
	else static if(is(T == interface))
		return TypeKind.interface_;
	else static if(isPrimitive!T)
		return TypeKind.primitive_;
	else
		static assert(0, "Unknown type passed in.");
}

private TypeInfo[] getInterfaces(T)() {
	static if(is(T == class) || is(T == interface)) {
		TypeInfo[] result = new TypeInfo[InterfacesTuple!T.length];
		foreach(i, type; InterfacesTuple!T) {
			result[i] = registerLazyLoader!type;
		}
		return result;
	} else
		return null;
}

private ProtectionLevel getProtection(T)() {
	static if(isPrimitive!T) {
		return ProtectionLevel.export_;
	} else {
		enum stringVal = __traits(getProtection, T);
		return to!ProtectionLevel(stringVal ~ "_");
	}
}

private ProtectionLevel getProtection(alias T)() {
	enum stringVal = __traits(getProtection, T);
	return to!ProtectionLevel(stringVal ~ "_");
}

private size_t getSize(T)() {
	static if(is(T == class))
		return __traits(classInstanceSize, T);
	else
		return T.sizeof;
}

private Variant convertTo(T)(TypeMetadata metadata, Variant instance, ConversionKind kind) {
	static if(is(Unqual!T == void)) {
		throw new ReflectionException("Unable to cast to void.");
	} else {
		if(kind == ConversionKind.coerce_) {
			static if(is(T == struct)) {
				// Support converting from void[] to a struct.
				// This is useful for creating instances of unknown structs.
				// For example, in our own createInstance method.
				void[]* arr = instance.peek!(void[]);
				if(arr) {
					T result = *(cast(T*)(*arr).ptr);
					return Variant(result);
				}
			} else static if(__traits(compiles, instance.coerce!T))  {
				return Variant(instance.coerce!T);
			}
		}
		// A coercion should support parsing the argument from a string.
		// BUG: An ICE prevents us from supporting arrays or associative arrays...
		// Can't reduce the ICE enough to file a bug report however.
		// Without the array / AA clause, the typeof(to!T(string.init)) will cause ICE at s2.ir:135.
		static if(!isArray!T && !isAssociativeArray!T && is(typeof(to!T(string.init)))) {
			if(kind == ConversionKind.coerce_ && !instance.convertsTo!T && instance.convertsTo!string) {
				string str = instance.get!string;
				T result = to!T(str);
				return Variant(result);
			}
		}
		return Variant(instance.as!T);
	}
}

private Variant invokeMethod(alias func, T, MethodParentKind parentType)(MethodMetadata metadata, void* instance, Variant[] args) {
	static if(variadicFunctionStyle!func == Variadic.no) {
		if(args.length != arity!func) {
			throw new ReflectionException("Expected " ~ arity!func.text ~ " arguments to "
										  ~ metadata.name ~ ", not " ~ args.length.text ~ ".");
		}
	}
	alias ArgTypes = staticMap!(ReplaceInout, ParameterTypeTuple!func);
	alias RetType = Unqual!(ReturnType!func);
	static struct Invoker {
		// Most sketchy hack.
		// We can't assign to params directly because it could be const.
		// So we have to do it inside a constructor, and thus make a fake struct for it.
		// Then can access the params below.
		this(Variant[] args) {
			enforce(args.length == ArgTypes.length);
			foreach(i, type; ArgTypes) {
				auto val = args[i].as!type(true);
				params[i] = val;
			}
		}
		ArgTypes params;
	}
	ArgTypes params = Invoker(args).params;
	//ParameterTypeTuple!func params = cast(ParameterTypeTuple!func)unqualParams;
	// TODO: Clean up return type stuff.
	static if(__traits(isStaticFunction, func)) {
		static if(!is(RetType == void))
			auto result = func(params);
		else
			func(params);
	} else {
		// Unqual to prevent inout issues.
		RetType delegate(ArgTypes) dg;
		static if(__traits(isVirtualMethod, func)) {
			// If this is a virtual method we'll have to handle dispatching it manually.
			enforce(metadata.vtblSlot > 0, "Attempting to call a virtual function with no vtable index computable.");
			size_t thisOffset;
			void* funcPtr = getVirtualFunctionPointer!(parentType)(instance, typeid(T), metadata.vtblSlot, thisOffset);
		} else {
			void* funcPtr = cast(void*)&func;
			size_t thisOffset = 0;
		}
		dg.funcptr = cast(RetType function(ArgTypes))(funcPtr);
		dg.ptr = cast(void*)instance + thisOffset;
		static if(!is(RetType == void))
			auto result = dg(params);
		else
			dg(params);
	}
	static if(is(RetType == void))
		return Variant(null);
	else // Cast away from const to prevent Variant assignment issues. Quite sketchy.
		return Variant(cast()result);
}

private template ReplaceInout(T) {
	static if(is(T == inout))
		alias ReplaceInout = typeof(cast(const)T.init);
	else
		alias ReplaceInout = T;
}

private void* getVirtualFunctionPointer(MethodParentKind parentType)(void* instance, TypeInfo ti, size_t vtblSlot, out size_t thisOffset) {
	// If we're operating on a virtual function, we have to worry about vtables and such.
	auto obj = cast(Object)instance;
	if(obj is null)
		throw new ReflectionException("Virtual methods may only be invoked on an instance that is an Object.");
	ClassInfo ci = obj.classinfo;
	void*[] vtbl;
	// For classes, this is trivial; just get the slot returned by __traits(getVirtualIndex) in it's vtable.
	static if(parentType == MethodParentKind.class_) {
		vtbl = ci.vtbl;
		enforce(vtbl.length > vtblSlot && vtblSlot > 0);
		thisOffset = 0;
		return ci.vtbl[vtblSlot];
	} else static if(parentType == MethodParentKind.interface_) {
		// For interfaces this is a bit more complex.
		// First, we have to find the right instance of Interface which stores the vtbl for that ClassInfo.
		// That instance may not be on the ClassInfo that instance is, but rather a base class.
		// When we find it, we can get a pointer to that class' implementation using the same approach as for classes.
		// Aka, vtblSlot within the object.Interface instance's vtbl.
		// Unfortunately this won't handle overrides.
		// To handle overrides, we get the offset of the interface, and the void*** there is a pointer to our vtbl.
		// Also, interface context pointers are actually offset by the object.Interface.offset value.
		// So we have to handle that as well, and set thisOffset to that value.

		// TODO: We can optimize this easily.
		// Ultimately, we're only trying to find the Interface instance to get it's offset.
		// But if the object we're invoking the method on is an interface reference,
		// we can get the offset through that. At least, I assume we can.
		// Not sure how Foo (Interface) -> Bar : Foo -> DerivedBar : Bar -> DerivedDerived : Foo, DerivedBar would work.
		// Really though, the cost should be negligible compared to the performance hit of using Variants and such.
		TypeInfo_Interface typeInterface = cast(TypeInfo_Interface)ti;
		for(ClassInfo curr = ci; curr; curr = curr.base) {
			foreach(inter; curr.interfaces) {
				if(inter.classinfo == typeInterface.info) {
					void*** vtblPtr = cast(void***)(instance + inter.offset);
					vtbl = (*vtblPtr)[0..inter.vtbl.length];
					enforce(vtbl.length > vtblSlot);
					auto interPtr = vtbl[vtblSlot];
					thisOffset = inter.offset;
					return interPtr;
				}
			}
		}
		throw new ReflectionException("Unable to find the vtable to invoke this method.");
	} else {
		static assert(0, "Expected non-virtual method when not a class or interface parent.");
	}
}

private ImplicitConversionType getImplicitConversionType(in TypeInfo from, in TypeInfo to) {
	// TODO: I think inout is handled incorrectly here.
	// Inout is supposed to apply the same qualifier for every type in the argument list, isn't it?
	// So the first it binds to it remains as...
	// Which would make the below wrong as it considers only a single parameter, so multiple inout substitutes can exist.
	if(from == to)
		return ImplicitConversionType.exact_;
	// Aside from exact match, can conversion const, immutable, or non-const to inout.
	// Can convert from inout to const only.
	// Can convert from non-const to const.
	// Nothing can implicitly remove shared.
	// Note that TypeInfo_Shared, TypeInfo_Inout, and TypeInfo_Invariant, derive from TypeInfo_Const.
	if(cast(TypeInfo_Shared)to || cast(TypeInfo_Shared)from)
		return ImplicitConversionType.none_;
	auto totic = cast(TypeInfo_Const)to;
	// If to is non-const, nothing can implicitly const convert to it. Saves a bit of effort below.
	if(totic is null)
		return ImplicitConversionType.none_;
	auto fromtic = cast(TypeInfo_Const)from;
	// Already handled shared and inout above, so const yet not immutable means just const.
	if(cast(TypeInfo_Inout)fromtic) {
		if(cast(TypeInfo_Invariant)totic is null && totic && fromtic.next == totic.next)
			return ImplicitConversionType.inout_;
		// Otherwise is from is inout and to isn't simply const then nothing can convert to it.
		return ImplicitConversionType.none_;
	}
	// Now to is inout, can convert from in any remaining situation (aka not shared).
	if(cast(TypeInfo_Inout)totic && (totic.next == from || (fromtic && totic.next == fromtic.next)))
		return ImplicitConversionType.inout_;
	// Otherwise if neither are inout, from can convert to to iff from is non-const and to is const.
	if(fromtic is null && totic && cast(TypeInfo_Invariant)totic is null && from == totic.next)
		return ImplicitConversionType.const_;
	// If from is non-const or is inout we can implicitly convert it to const.
	if(totic && (totic.next == to || (fromtic && fromtic.next == totic.next)))
		return ImplicitConversionType.const_;
	return ImplicitConversionType.none_;
} 

@name("Implicit Conversion Type")
unittest {
	assert(getImplicitConversionType(typeid(int), typeid(int)) == ImplicitConversionType.exact_);
	assert(getImplicitConversionType(typeid(inout int), typeid(inout int)) == ImplicitConversionType.exact_);
	assert(getImplicitConversionType(typeid(int), typeid(const int)) == ImplicitConversionType.const_);
	assert(getImplicitConversionType(typeid(int), typeid(inout int)) == ImplicitConversionType.inout_);
	assert(getImplicitConversionType(typeid(const int), typeid(inout int)) == ImplicitConversionType.inout_);
	assert(getImplicitConversionType(typeid(immutable int), typeid(inout int)) == ImplicitConversionType.inout_);
	assert(getImplicitConversionType(typeid(inout int), typeid(const int)) == ImplicitConversionType.inout_);
	assert(getImplicitConversionType(typeid(inout int), typeid(immutable int)) == ImplicitConversionType.none_);
	assert(getImplicitConversionType(typeid(int), typeid(string)) == ImplicitConversionType.none_);
}

private Variant getFieldValue(InstanceType, size_t fieldIndex)(ValueMetadata metadata, Variant instanceWrapper) {
	InstanceType instance = instanceWrapper.as!InstanceType;
	// TODO: We can likely reduce template bloat if we can make the below work at compile-time instead of fieldIndex.
	// For example, by manually making a switch statement that gets the right value.
	// That might create more bloat than it saves though, not sure...
	// For now, this works fine.
	//auto result = instance.tupleof[metadata.fieldData.index];
	auto result = instance.tupleof[fieldIndex];
	return Variant(result);
}

private void setFieldValue(InstanceType, size_t fieldIndex)(ValueMetadata metadata, Variant instanceWrapper, Variant valueWrapper) {
	// TODO: Same as for getFieldValue. Remove fieldIndex template param.
	//size_t fieldIndex = metadata.fieldData.index;
	InstanceType* instance = unwrapVariantPointer!InstanceType(instanceWrapper);
	if(instance is null)
		throw new ReflectionException("Unable to set a value of a struct that was not passed by reference or pointer.");
	static if(is(InstanceType == const) || is(InstanceType == immutable))
		throw new ReflectionException("Unable to modify const or immutable instance.");
	else
		instance.tupleof[fieldIndex] = valueWrapper.as!(typeof(instance.tupleof[fieldIndex]));
}

private Variant getPropertyValue(InstanceType)(ValueMetadata metadata, Variant instanceWrapper) {
	auto method = metadata.propertyData.getter;
	if(method == MethodMetadata.init)
		throw new ReflectionException("No getter was found for this method.");
	return method.invoke!(InstanceType)(instanceWrapper.as!(InstanceType));
}

private void setPropertyValue(InstanceType)(ValueMetadata metadata, Variant instanceWrapper, Variant valueWrapper) {
	auto setters = metadata.propertyData.setters;
	auto method = findMethodInternal(setters, metadata.symbol.name, [valueWrapper.type]);
	if(method == MethodMetadata.init)
		throw new ReflectionException("Unable to find a setter that takes in a " ~ valueWrapper.type.text ~ ".");
	InstanceType* inst = unwrapVariantPointer!InstanceType(instanceWrapper);
	if(inst is null)
		throw new ReflectionException("Unable to set a property on a struct that was not passed by reference or pointer.");
	static if(is(InstanceType == const) || is(InstanceType == immutable))
		throw new ReflectionException("Unable to modify const or immutable instance.");
	else
		method.invokeInternal!(InstanceType)(inst, valueWrapper);
}

private Variant getEnumConstant(T, string m)(ValueMetadata unused, Variant instance) {
	// NOTE: cast instead of to, in order to prevent to!string(enum) which is wrong if base is string.
	return Variant(cast(OriginalType!T)__traits(getMember, T, m));
}

// Helper to allow extensions to Variant get without going all the way to coerce.
// For example, if FooBar derives from Foo and we have a variant storing Foo, we want
// to be able to get it as an instance FooBar without risking a full coercion and string
// conversions / parsing / such.
// If stripConst is set and the stored value is const, inout, or immutable, it will be retrieved as T instead.
// Note that shared is excluded and if T is shared stripConst will be ignored.
private T as(T)(Variant inst, bool stripConst = false) {
	// Variant does not allow getting or coercing when it stores null.
	if(inst.type == typeid(null)) {
		/+// Hack to work around compiler bug causing this to try to do TypeInfo.init() which fails due to not being static.
		// This is not scalable though because any type could define 'init' in theory.
		//static if(is(T : TypeInfo))
		static if(is(T == class) || is(T == interface))
		return null;
		else
		return T.init;+/
		// Hack to work around compiler bug causing linker errors when using T.init, as well as the above hack for TypeInfo.init.
		T result;
		return result;
	}
	static if(!is(T == shared)) {
		if(stripConst) {
			TypeInfo type = inst.type;
			static if(__traits(compiles, cast(T)asHelper!(immutable T)(inst))) {
				if(cast(TypeInfo_Invariant)type)
					return cast(T)asHelper!(immutable T)(inst);
			}
			static if(__traits(compiles, cast(T)asHelper!(const T)(inst))) {
				if(cast(TypeInfo_Const)type)
					return cast(T)asHelper!(const T)(inst);
			}
		}
	}
	return asHelper!T(inst);
}

private T asHelper(T)(Variant inst) {
	static if(is(typeof(inst.coerce!T))) {
		if(cast(ClassInfo)inst.type) {
			// We only do this for classes.
			return inst.coerce!T;
		}
	}
	return inst.get!T;
}

// If a Variant, returns the Variant.
// If a struct, returns a Variant containing a pointer to the instance.
// Otherwise, returns a Variant containing the instance (not a pointer to it).
private Variant wrapVariantPointer(InstanceType)(ref InstanceType val) {
	static if(isVariant!InstanceType)
		return val;
	else static if(is(InstanceType == struct))
		return Variant(&val);
	else
		return Variant(val);
}

// Undoes the wrapping by wrapVariantPointer.
// If a cast to a pointer or class instance is not possible, a ReflectionException is thrown.
// This occurs in the case of val not storing a class, interface, or InstanceType*.
private InstanceType* unwrapVariantPointer(InstanceType)(Variant val) {
	if(val.type == typeid(InstanceType*))
		return val.get!(InstanceType*);
	static if(is(InstanceType == class) || is(InstanceType == interface)) {
		// Peek is not safe as it's tied to the lifetime of the Variant (aka this scope).
		//return val.peek!InstanceType;
		// TODO: Find a better solution for this or change this to not guarantee a pointer.
		// And then handle classes differently.
		InstanceType* ptr = cast(InstanceType*)GC.malloc((InstanceType*).sizeof);
		*ptr = val.as!InstanceType;
		return ptr;
	} else
		return null;
}

private TypeInfo[] templateArgsToTypeInfo(ArgTypes...)() {
	TypeInfo[] paramTypes = new TypeInfo[ArgTypes.length];
	foreach(i, argType; ArgTypes)
		paramTypes[i] = typeid(argType);
	return paramTypes;
}

private __gshared StoredTypeMetadata[TypeInfo] _typeData;
private __gshared StoredModuleMetadata[string] _moduleData;

private struct StoredTypeMetadata {
	TypeMetadata function() loader;
	TypeMetadata metadata;
	bool isLoaded;

	void load() {
		assert(!isLoaded);
		this.metadata = loader();
		this.loader = null;
		this.isLoaded = true;
	}

	this(TypeMetadata function() loader) {
		this.loader = loader;
		this.metadata = TypeMetadata.init;
		this.isLoaded = false;
	}

	this(TypeMetadata metadata) {
		this.metadata = metadata;
		this.isLoaded = true;
		this.loader = null;
	}
}

private struct StoredModuleMetadata {
	ModuleMetadata delegate() loader;
	string moduleName;
}

// If metadata for the given type info exists, it will be lazily loaded and returned; otherwise init.
private TypeMetadata getStoredExisting(TypeInfo typeInfo, bool skipLoad) {
	if(auto res = typeInfo in _typeData) {
		if(!res.isLoaded) {
			if(skipLoad)
				return TypeMetadata.init;
			version(logreflect) writeln("Metadata for " ~ typeInfo.text ~ " was stored but not loaded; loading now.");
			res.load();
		}
		return res.metadata;
	}
	return TypeMetadata.init;
}

// Helper template to allow us to alias __traits(getMember, T, m).
private template aliasSelf(T...) if(T.length == 1) {
	alias T aliasSelf;
}

private template isPrimitive(T) {
	enum isPrimitive = (isBuiltinType!T || isArray!T || isPointer!T || is(T == function) || is(T == delegate));
}

private template isType(T...) if(T.length == 1) {
	enum isType = is(T[0] == class) || is(T[0] == struct) || is(T[0] == union) || is(T[0] == enum) || is(T[0] == interface);
}

private template isField(alias T) {
	enum isField = hasField!(__traits(parent, T), __traits(identifier, T));
}

private template isStaticConstructor(alias T) {
	enum isStaticConstructor =
		__traits(identifier, T).startsWith("_sharedStaticCtor") ||
			__traits(identifier, T).startsWith("_staticCtor");
}

private template isVariant(T) {
	// TODO: Need a real way of checking.
	enum isVariant = is(Unqual!T == Variant);
}

/// Returns the index for the field with the given name on type $(D T).
template fieldIndex(T, string field) {
	static if(is(T == interface))
		enum fieldIndex = -1;
	else {
		enum fieldIndex = fieldIndexImpl!(T, field, 0);
	}
}

private template fieldIndexImpl(T, string field, size_t i) {
	static if(is(T == enum)) {
		static if(__traits(allMembers, T).length == i)
			enum fieldIndexImpl = -1;
		else static if(__traits(allMembers, T)[i] == field)
			enum fieldIndexImpl = i;
		else
			enum fieldIndexImpl = fieldIndexImpl!(T, field, i + 1);
	} else static if (T.tupleof.length == i)
		enum fieldIndexImpl = -1;
	else static if(T.tupleof[i].stringof == field)
		enum fieldIndexImpl = i;
	else
		enum fieldIndexImpl = fieldIndexImpl!(T, field, i + 1);
}

private string getIsFlagsMixin(T, string flagsName)() if(is(T == enum)) {
	string result = "";
	foreach(m; __traits(allMembers, T)) {
		string transformedName = capitalize(m);
		if(transformedName[$-1] == '_')
			transformedName = transformedName[0..$-1];
		result ~= "/// Indicates if this symbol has the $(D" ~ m[0..$-1] ~ ") modifier.\r\n";
		result ~= "/// This method is simply a shortcut to check if the bit is set in $(D modifiers).\r\n";
		result ~= "@property bool is" ~ transformedName ~ "() @safe const pure nothrow {";
		result ~= "\r\n\treturn (" ~ flagsName ~ " & " ~ T.stringof ~ "." ~ m ~ ") != 0;\r\n}";
	}
	return result;
}

version(unittest) {
	struct ReflectionTestStruct {
		string stringVal;

		@property string stringValProperty() const {
			return stringVal;
		}
	}

	struct ReflectionTestAttribute {
		int val;
		this(int val) {
			this.val = val;
		}
	}

	struct ReflectionLargeStructTester {
		ubyte[16] unusedPadding;
		ubyte[Variant.size] maxVariantPadding;
		int _val;

		this(int val) {
			_val = val;
		}

		@property int val() const {
			return _val;
		}

		@property void val(int val) {
			_val = val;
		}

		int returnDouble() const {
			return val * 2;
		}
	}

	struct ReflectionFloatStructTester {
		int a;
		float b;
	}

	interface ReflectionTestInterface {
		@property int val() const;
	}

	class ReflectionTestClass : ReflectionTestInterface {
		private int _val = 3;

		@property int val() const {
			return _val;
		}

		@property void val(int value) {
			_val = value;
		}

		int foo(int x = 1) {
			return x;
		}

		void doubleRef(lazy scope int x) {
			int tmp = x();
			tmp *= 2;
		}

		static int staticMethod() {
			return 3;
		}

		this() {

		}

		this(int val) {
			this._val = val;
		}
	}

	class ReflectionDerivedClass : ReflectionTestClass {
		int derivedField;

		@property override int val() const {
			return super.val * 2;
		}

		@property override void val(int val) {
			super.val = val;
		}

		int bar(int x) {
			return x * 2;
		}

		override int foo(scope int x = 2) {
			return x * 10;
		}

	}

	class ReflectionUdaTest {
		@ReflectionTestAttribute(3) int val;

		// TODO: This results in errors in std.traits.DefaultValueTuple when getting parameters for check.
		//@CheckerAttribute!(c)() int checkedVal;

		@(6) @property int valProp() const {
			return val;
		}
	}

	/+bool c(int val) {
	return val > 3;
	}

	struct CheckerAttribute(alias fun) {
	bool check(int args) {
	return unaryFun!fun(args);
	}
	}+/

	enum ReflectionTestEnum {
		a, b, c
	}

	enum ReflectionTestBaseEnum : string {
		myVal = "mv",
		myVal2 = "mv2",
		myValDup = "mv"
	}

	class ReflectionTestNestedClass {
		int a;
		class Nested {
			int _b;
			this(int b) {
				this._b = b;
			}

			@property int b() const {
				return _b;
			}

			int returnDouble() const {
				return b * 2;
			}
		}
	}

	// Full functionality tests.

	// Basic struct tester:
	@name("Struct Metadata")
		unittest {
			TypeMetadata metadata = createMetadata!ReflectionTestStruct;
			assert(metadata.kind == TypeKind.struct_);
			ReflectionTestStruct instance = ReflectionTestStruct();
			instance.stringVal = "abc";
			auto children = metadata.children;
			assert(children.types.length == 0);
			assert(children.methods.length == 0);
			assert(children.values.length == 2);
			ValueMetadata field = metadata.findValue("stringVal");
			assert(field.name == "stringVal");
			assert(field.protection == ProtectionLevel.public_);
			assert(field.attributes.length == 0);
			assert(field.getValue(instance) == "abc");
			assert(field.type == typeid(string));
			field.setValue(instance, "def");
			assert(field.getValue(instance) == "def");
			assert(field.getValue(instance).type == typeid(string));
			assert(instance.stringVal == "def");
			ValueMetadata prop = metadata.findValue("stringValProperty");
			assert(prop.getValue(instance) == "def");
			assert(prop.propertyData.getter.name == "stringValProperty");

			auto floatTesterMeta = createMetadata!ReflectionFloatStructTester;
			auto inst2 = floatTesterMeta.createInstance.get!ReflectionFloatStructTester;
			assert(inst2.a == 0);
			assert(isNaN(inst2.b));

			// TODO: Support createInstance with args for structs.
			/+auto attribTesterMeta = createMetadata!ReflectionTestAttribute;
			auto inst4 = attribTesterMeta.createInstance(6).get!ReflectionTestAttribute;
			assert(inst4.val == 6);

			auto largeTesterMeta = createMetadata!ReflectionLargeStructTester;
			auto inst3 = largeTesterMeta.createInstance(4).get!ReflectionLargeStructTester;
			assert(inst3.val == 4);+/
		}

	// Basic class tester:
	@name("Class Metadata")
		unittest {
			TypeMetadata metadata = createMetadata!ReflectionTestClass;
			assert(metadata.base == typeid(Object));
			assert(metadata.kind == TypeKind.class_);
			assert(metadata.parent is null);
			ReflectionTestClass instance = new ReflectionTestClass();
			auto children = metadata.children;
			auto firstMethod = metadata.findMethod("foo", [typeid(int)]);
			assert(firstMethod != MethodMetadata.init);
			assert(firstMethod.name == "foo");
			assert(firstMethod.returnType == typeid(int));
			assert(firstMethod.invoke(instance, 3) == 3);
			assert(metadata.invokeMethod("foo", instance, 4) == 4);
			assert(metadata.findMethod("val") == MethodMetadata.init);
			ValueMetadata val = metadata.findValue("val");
			assert(val != ValueMetadata.init);
			assert(val.getValue(instance) == 3);
			val.setValue(instance, 6);
			assert(val.getValue(instance) == 6);
			auto newInst = metadata.createInstance(200).get!ReflectionTestClass;
			assert(newInst.val == 200);
			auto valBackingField = metadata.findValue("_val");
			assert(valBackingField.name == "_val");
			assert(valBackingField.protection == ProtectionLevel.private_);
			assert(valBackingField.fieldData.declaringType == typeid(ReflectionTestClass));
			MethodMetadata doubler = metadata.findMethod("doubleRef", typeid(int));
			assert(doubler.name == "doubleRef");
			assert(doubler.symbol.protection == ProtectionLevel.public_);
			assert(doubler.parameters.length == 1);
			assert(doubler.returnType == typeid(void));
			auto doublerParam = doubler.findParameter("x");
			assert(doublerParam.name == "x");
			assert(!doublerParam.hasDefaultValue);
			assert(doublerParam.modifiers == (ParameterStorageClass.lazy_ | ParameterStorageClass.scope_));
			MethodMetadata staticMethod = metadata.findMethod("staticMethod");
			assert(staticMethod.name == "staticMethod");
			assert(staticMethod.invoke(null) == 3);


			// Test derived classes:
			auto derivedData = createMetadata!ReflectionDerivedClass;
			Variant derived = derivedData.createInstance(); // Test as Variant.
			assert(derived.metadata == derivedData);
			assert(derivedData.parent is null);
			assert(derivedData.base == typeid(ReflectionTestClass));
			MethodMetadata fooData = derivedData.findMethod("foo", typeid(int));
			assert(fooData.name == "foo");
			assert(fooData.protection == ProtectionLevel.public_);
			assert(fooData.parameters.length == 1);
			ParameterMetadata param = fooData.findParameter("x");
			assert(param.name == "x");
			assert(param.hasDefaultValue);
			assert(param.defaultValue == 2);
			assert(param.modifiers & ParameterStorageClass.scope_);
			assert(param.type == typeid(int));
			assert(fooData.invoke(derived, 4) == 40);
			assert(derivedData.findMethod("foo", typeid(string)) == MethodMetadata.init);
			assert(derivedData.findMethod("foobar", typeid(string)) == MethodMetadata.init);
			ValueMetadata derivedField = derivedData.findValue("derivedField");
			assert(derivedField.name == "derivedField");
			assert(derivedField.kind == DataKind.field);
			auto derivedFieldData = derivedField.fieldData;
			assert(derivedFieldData.declaringType == typeid(ReflectionDerivedClass));
			assert(derivedFieldData.index == 0);
			ValueMetadata nonDerivedField = derivedData.findValue("_val");
			assert(nonDerivedField.name == "_val");
			assert(nonDerivedField.fieldData.declaringType == typeid(ReflectionTestClass));
			assert(metadata.findMethod("staticMethod") == staticMethod);
			ValueMetadata derivedVal = derived.metadata.findValue("val");
			assert(derivedVal.getValue(derived) == 6);
			derivedVal.setValue(derived, 9);
			assert(derivedVal.getValue(derived) == 18);
		}

	// Interface tests:
	@name("Interface Metadata")
		unittest {
			TypeMetadata metadata = createMetadata!ReflectionTestInterface;
			ReflectionTestInterface instance = new ReflectionTestClass(10);
			auto val = metadata.findValue("val");
			assert(val.getValue(instance) == 10);
			auto clsMeta = createMetadata!ReflectionTestClass;
			assert(clsMeta.interfaces.length == 1);
			assert(clsMeta.interfaces[0] == typeid(ReflectionTestInterface));
			auto derived = new ReflectionDerivedClass();
			assert(val.getValue(derived) == 6);
		}

	// Enum tests:
	@name("Enum Metadata")
		unittest {
			TypeMetadata metadata = createMetadata!ReflectionTestEnum;
			assert(metadata.name == "ReflectionTestEnum");
			assert(metadata.kind == TypeKind.enum_);
			assert(metadata.children.values.length == 3);
			ValueMetadata val = metadata.findValue("a");
			assert(val.fieldData.index == 0);
			assert(metadata.findValue("b").fieldData.index == 1);
			assert(metadata.findValue("c").fieldData.index == 2);
			assert(val.name == "a");
			assert(val.getValue(null) == 0);
			assert(metadata.getValue("b", null) == 1);
			assert(metadata.getValue("c", null) == 2);

			TypeMetadata baseData = createMetadata!ReflectionTestBaseEnum;
			assert(baseData.base == typeid(string));
			assert(baseData.getValue("myVal", null) == "mv");
			ValueMetadata myVal = baseData.findValue("myVal");
			ValueMetadata myValDup = baseData.findValue("myValDup");
			assert(myVal.getValue(null) == myValDup.getValue(null));
			assert(myVal.kind == DataKind.constant);
			assert(myVal.getValue(null) == "mv");
			assert(myVal.fieldData.index == 0);
			assert(myValDup.fieldData.index == 2);

		}

	// UDA tests
	@name("UDA Metadata")
		unittest {
			auto metadata = createMetadata!ReflectionUdaTest;
			auto field = metadata.findValue("val");
			assert(field != ValueMetadata.init);
			assert(field.attributes.length == 1);
			assert(field.hasAttribute(typeid(ReflectionTestAttribute)));
			assert(!field.hasAttribute(typeid(int)));
			assert(field.findAttribute!int == 0);
			auto attr = field.findAttribute!ReflectionTestAttribute;
			assert(attr == ReflectionTestAttribute(3));
			assert(field.findAttribute!ReflectionTestAttribute(6) == 3);
			assert(field.findAttribute!(ReflectionUdaTest)(9) == 9);
			auto prop = metadata.findValue("valProp");
			Symbol getterSymbol = prop.propertyData.getter;
			assert(getterSymbol.hasAttribute(typeid(int)));
			assert(getterSymbol.findAttribute!int == 6);
			/+auto checker = metadata.findValue("checkedVal").attributes[0];
			auto t = __traits(getAttributes, ReflectionUdaTest.checkedVal)[0];
			writeln(t.check(3));
			writeln("metadata = ", checker);
			writeln("type = ", checker.type);
			auto checkerData = checker.type.metadata;
			assert(checkerData.invokeMethod("check", checker, 4).get!bool);+/
		}

	// Nested type tests
	@name("Nested types [disabled]")
		unittest {
			auto parentData = createMetadata!ReflectionTestNestedClass;
			assert(parentData.children.values.length == 1);
			assert(parentData.findValue("a") != ValueMetadata.init);
			assert(parentData.findValue("b") == ValueMetadata.init);
			assert(parentData.name == "ReflectionTestNestedClass");
			// TODO: This is bugged, fix it.
			/+assert(parentData.children.types.length == 1, "Expected one child type, got " ~ parentData.children.types.length.text ~ ".");
			auto metadata = parentData.findType("Nested");
			assert(metadata.name == "Nested");
			assert(metadata.children.values.length == 2);
			auto instance = metadata.createInstance(5);
			assert(metadata.getValue("_b", instance) == 5);
			assert(metadata.invokeMethod("returnDouble", instance) == 10);+/
		}

	// Test various things with variants.
	@name("Variants")
		unittest {
			auto structData = createMetadata!ReflectionTestStruct;
			ReflectionTestStruct s = ReflectionTestStruct("abc");
			Variant v = s;
			assert(v.metadata == structData);
			ValueMetadata stringVal = structData.findValue("stringVal");
			assert(stringVal != ValueMetadata.init);
			assert(stringVal.getValue(v) == "abc");

			auto derivedData = createMetadata!ReflectionDerivedClass;
			ReflectionTestInterface testInter = new ReflectionDerivedClass();
			Variant interVar = testInter;
			assert(derivedData.findMethod("foo", typeid(int)) != MethodMetadata.init);
			assert(derivedData.invokeMethod("foo", interVar, 5) == 50);

			auto largeData = createMetadata!ReflectionLargeStructTester;
			Variant largeVar = ReflectionLargeStructTester(4);
			assert(largeData == largeVar.metadata);
			assert(largeData.getValue("val", largeVar) == 4);
			assert(largeData.invokeMethod("returnDouble", largeVar) == 8);

			// Statically assign a type, while actually storing the derived type.
			ReflectionTestClass inst = new ReflectionDerivedClass();
			Variant derivedVar = inst;
			assert(derivedVar.metadata.type == typeid(ReflectionDerivedClass));
		}

	// Test dynamic type casting.
	@name("Dynamic Type Casting")
		unittest {
			auto intMetadata = createMetadata!int;
			Variant initial = "3";
			assert(intMetadata.coerceFrom(initial) == 3);
			assertThrown(intMetadata.castFrom(initial));
			assert(intMetadata.castFrom(Variant(cast(short)6)) == 6);

			ReflectionTestInterface rti = new ReflectionTestClass();
			auto metaRti = createMetadata!ReflectionTestInterface;
			assertNotThrown(metaRti.castFrom(new ReflectionTestClass()));
			assertNotThrown(metaRti.coerceFrom(new ReflectionTestClass()));

			auto metaRte = createMetadata!ReflectionTestEnum;
			assert(metaRte.coerceFrom("b") == ReflectionTestEnum.b);
			auto metaRtbe = createMetadata!ReflectionTestBaseEnum;
			assert(metaRtbe.coerceFrom("myVal2") == ReflectionTestBaseEnum.myVal2);

			// TODO: This needs more tests. Particularly object -> interface -> base.
		}

	// Test type qualifiers.
	@name("Type Qualifiers")
		unittest {
			static class Foo { }
			inout(Foo) foo(inout Foo f) { 
				assert(f.metadata.qualifiers & TypeQualifier.inout_);
				return f; 
			}
			const Foo a;
			Foo b;
			shared Foo c;
			const shared Foo d;
			assert(a.metadata.qualifiers & TypeQualifier.const_);
			assert(a.metadata.qualifiers == foo(a).metadata.qualifiers);
			assert(b.metadata.qualifiers == TypeQualifier.none_);
			assert(c.metadata.qualifiers & TypeQualifier.shared_);
			assert(d.metadata.qualifiers & (TypeQualifier.shared_ | TypeQualifier.const_));
		}

	// Test symbol modifiers.
	@name("Symbol Modifiers")
		unittest {
			class Foo {
				void foo() { }
				static void foo(int a) { }
				@property static int bar() { return 1; }
			}
			// At the moment, only static is implemented.
			// And that only for methods...
			auto metadata = createMetadata!Foo;
			assert(metadata.findMethod("foo").modifiers == SymbolModifier.none_);
			assert(metadata.findMethod("foo", typeid(int)).modifiers == SymbolModifier.static_);
			assert(metadata.findValue("bar").modifiers == SymbolModifier.static_);
			assert(metadata.findValue("bar").isStatic);
			assert(metadata.getValue("bar", null) == 1);
		}

	@name("Method Implicit Conversions")
		unittest {
			class Foo {
				int foo(Foo f) { return 0; }
				int foo(inout Foo f) { return 1; }
				int foo(const Foo f) { return 2; }
				int bartest() { return -1; }
			}
			import std.stdio;
			auto metadata = createMetadata!Foo;
			MethodMetadata mutableData = metadata.findMethod("foo", typeid(Foo));
			MethodMetadata constData = metadata.findMethod("foo", typeid(const Foo));
			MethodMetadata immutableData = metadata.findMethod("foo", typeid(immutable Foo));
			assert(mutableData == metadata.children.methods[0]);
			assert(immutableData == metadata.children.methods[1]);
			assert(constData == metadata.children.methods[2]);
			Foo inst = new Foo();
			assert(mutableData.invoke(inst, inst) == 0);
			assert(immutableData.invoke(inst, cast(immutable)inst) == 1);
			assert(constData.invoke(inst, cast(const)inst) == 2);
			assert(metadata.findMethod("foo", typeid(shared int)) == MethodMetadata.init);
			assert(metadata.findMethod("foo", typeid(const shared int)) == MethodMetadata.init);
			assert(metadata.invokeMethod("foo", inst, cast(immutable)inst) == 1);
		}

	// Test module header example.
	// Would be nice if documented unittests worked on module headers.
	@name("Module Documentation")
		unittest {
			class Foo {
				int _val;
				this(int val) {
					this._val = val;
				}

				@property int val() const {
					return _val;
				}

				@property void val(int value) {
					_val = value;
				}

				int getSquare(int input) {
					return input * input;
				}
			}

			// First, we need to generate metadata for Foo.
			auto metadata = createMetadata!Foo;
			// Then we can create an instance.
			// Note that creating an instance returns a Variant, so the result must be converted.
			Variant varInstance = metadata.createInstance(3);
			Foo instance = varInstance.get!Foo;
			// Can easily get any property or field through getValue.
			assert(metadata.getValue("val", instance) == 3);
			// Since the type may not be known at compile-time, we can also operate on the Variant directly.
			// Note that in this scenario the metadata will have to be created through createMetadata prior to this.
			// We created it above with createMetadata!Foo, so we're fine.
			assert(metadata == varInstance.metadata);
			assert(metadata.getValue("val", varInstance) == 3);
			// Of course, can set values and invoke methods as well.
			metadata.setValue("val", instance, 6);
			assert(metadata.getValue("val", instance) == 6);
			assert(metadata.invokeMethod("getSquare", instance, 4) == 16);
		}
}



