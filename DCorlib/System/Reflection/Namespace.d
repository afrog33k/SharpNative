module System.Reflection.Namespace;
import System.Namespace;
import System.__Internal.Namespace;
import System.Reflection.Internal;
import System.Globalization.Namespace;
import std.traits;
import std.string;
import std.array;
import std.stdio;
import core.vararg;
alias typeof(null) null_t;


class AmbiguousMatchException : SystemException {

	private static const E_AMBIGUOUSMATCH = "Ambiguous match found";

	this() {
		super(new String(E_AMBIGUOUSMATCH));
	}


	this(String paramName) {
		super(_S(E_AMBIGUOUSMATCH));
	}

	

}




public class ParameterInfo: NObject
{

}

public class Signature : NObject
{
}

public enum MethodAttributes
{
	// NOTE: This Enum matchs the CorMethodAttr defined in CorHdr.h

	// member access mask - Use this mask to retrieve accessibility information.
	MemberAccessMask    =   0x0007,
        PrivateScope        =   0x0000,     // Member not referenceable.
        Private             =   0x0001,     // Accessible only by the parent type.  
        FamANDAssem         =   0x0002,     // Accessible by sub-types only in this Assembly.
        Assembly            =   0x0003,     // Accessibly by anyone in the Assembly.
        Family              =   0x0004,     // Accessible only by type and sub-types.    
        FamORAssem          =   0x0005,     // Accessibly by sub-types anywhere, plus anyone in assembly.
        Public              =   0x0006,     // Accessibly by anyone who has visibility to this scope.    
        // end member access mask

        // method contract attributes.
        Static              =   0x0010,     // Defined on type, else per instance.
        Final               =   0x0020,     // Method may not be overridden.
        Virtual             =   0x0040,     // Method virtual.
        HideBySig           =   0x0080,     // Method hides by name+sig, else just by name.
        CheckAccessOnOverride=  0x0200,

        // vtable layout mask - Use this mask to retrieve vtable attributes.
        VtableLayoutMask    =   0x0100,
        ReuseSlot           =   0x0000,     // The default.
        NewSlot             =   0x0100,     // Method always gets a new slot in the vtable.
        // end vtable layout mask

        // method implementation attributes.
        Abstract            =   0x0400,     // Method does not provide an implementation.
        SpecialName         =   0x0800,     // Method is special.  Name describes how.

        // interop attributes
        PinvokeImpl         =   0x2000,     // Implementation is forwarded through pinvoke.
        UnmanagedExport     =   0x0008,     // Managed method exported via thunk to unmanaged code.
        RTSpecialName       =   0x1000,     // Runtime should check name encoding.

        // Reserved flags for runtime use only.
        ReservedMask              =   0xd000,
        HasSecurity               =   0x4000,     // Method has security associate with it.
        RequireSecObject          =   0x8000,     // Method calls another method containing security code.
}
public enum MethodImplAttributes
{
	// code impl mask
	CodeTypeMask       =   0x0003,   // Flags about code type.   
        IL                 =   0x0000,   // Method impl is IL.
        Native             =   0x0001,   // Method impl is native.     
        /// <internalonly/>
        OPTIL              =   0x0002,   // Method impl is OPTIL 
        Runtime            =   0x0003,   // Method impl is provided by the runtime.
        // end code impl mask

        // managed mask
        ManagedMask        =   0x0004,   // Flags specifying whether the code is managed or unmanaged.
        Unmanaged          =   0x0004,   // Method impl is unmanaged, otherwise managed.
        Managed            =   0x0000,   // Method impl is managed.
        // end managed mask

        // implementation info and interop
        ForwardRef         =   0x0010,   // Indicates method is not defined; used primarily in merge scenarios.
        PreserveSig        =   0x0080,   // Indicates method sig is exported exactly as declared.

        InternalCall       =   0x1000,   // Internal Call...

        Synchronized       =   0x0020,   // Method is single threaded through the body.
        NoInlining         =   0x0008,   // Method may not be inlined.
     //   [System.Runtime.InteropServices.ComVisible(false)]
        AggressiveInlining =   0x0100,   // Method should be inlined if possible.
        NoOptimization     =   0x0040,   // Method may not be optimized.

        MaxMethodImplVal   =   0xFFFF,   // Range check value
}

public enum FieldAttributes
{
	// member access mask - Use this mask to retrieve accessibility information.
		FieldAccessMask         =    0x0007,
        PrivateScope            =    0x0000,    // Member not referenceable.
        Private                 =    0x0001,    // Accessible only by the parent type.  
        FamANDAssem             =    0x0002,    // Accessible by sub-types only in this Assembly.
        Assembly                =    0x0003,    // Accessibly by anyone in the Assembly.
        Family                  =    0x0004,    // Accessible only by type and sub-types.    
        FamORAssem              =    0x0005,    // Accessibly by sub-types anywhere, plus anyone in assembly.
        Public                  =    0x0006,    // Accessibly by anyone who has visibility to this scope.    
        // end member access mask

        // field contract attributes.
        Static                  =    0x0010,        // Defined on type, else per instance.
        InitOnly                =    0x0020,     // Field may only be initialized, not written to after init.
        Literal                 =    0x0040,        // Value is compile time constant.
        NotSerialized           =    0x0080,        // Field does not have to be serialized when type is remoted.

        SpecialName             =    0x0200,     // field is special.  Name describes how.

        // interop attributes
        PinvokeImpl             =    0x2000,        // Implementation is forwarded through pinvoke.

        // Reserved flags for runtime use only.
        ReservedMask            =   0x9500,
        RTSpecialName           =   0x0400,     // Runtime(metadata internal APIs) should check name encoding.
        HasFieldMarshal         =   0x1000,     // Field has marshalling information.
        HasDefault              =   0x8000,     // Field has default.
        HasFieldRVA             =   0x0100,     // Field has RVA.
}

public enum MemberTypes
{
	// The following are the known classes which extend MemberInfo
		Constructor     = 0x01,
        Event           = 0x02,
        Field           = 0x04,
        Method          = 0x08,
        Property        = 0x10,
        TypeInfo        = 0x20,
        Custom          = 0x40,
        NestedType      = 0x80,
        All             = Constructor | Event | Field | Method | Property | TypeInfo | NestedType,
}

public enum CallingConventions
{
	//NOTE: If you change this please update COMMember.cpp.  These
	//    are defined there.
		Standard        = 0x0001,
        VarArgs         = 0x0002,
        Any             = Standard | VarArgs,
        HasThis         = 0x0020,
        ExplicitThis    = 0x0040,
}

public class Binder : NObject
{
}

public class MemberInfo : NObject
{
	public String Name; // Will change these to properties later
	

	public this()
	{

	}

	public abstract MemberTypes MemberType() @property;

	public  Type DeclaringType;

	public  Type ReflectedType;

/*public IEnumerable__G!(CustomAttributeData) CustomAttributes()
	{
		
			return GetCustomAttributesData();
		
	}*/
	public abstract NObject[] GetCustomAttributes(bool inherit);

	public abstract NObject[] GetCustomAttributes(Type attributeType, bool inherit);

	public abstract bool IsDefined(Type attributeType, bool inherit);

/*	public IList__G!(CustomAttributeData) GetCustomAttributesData()
	{
		throw new NotImplementedException();
	}*/

	public int MetadataToken()   { throw new InvalidOperationException(); }

	/*publicModule Module
	{ 
		get
		{
			if (this is Type)
				return ((Type)this).Module;

			throw new NotImplementedException(); 
		} 
	}
*/


	
	/*#if !FEATURE_CORECLR
	public static bool operator ==(MemberInfo left, MemberInfo right)
	{
		if (ReferenceEquals(left, right))
			return true;

		if ((object)left == null || (object)right == null)
			return false;

		Type type1, type2;
		MethodBase method1, method2;
		FieldInfo field1, field2;
		EventInfo event1, event2;
		PropertyInfo property1, property2;

		if ((type1 = left as Type) != null && (type2 = right as Type) != null)
			return type1 == type2;
		else if ((method1 = left as MethodBase) != null && (method2 = right as MethodBase) != null)
			return method1 == method2;
		else if ((field1 = left as FieldInfo) != null && (field2 = right as FieldInfo) != null)
			return field1 == field2;
		else if ((event1 = left as EventInfo) != null && (event2 = right as EventInfo) != null)
			return event1 == event2;
		else if ((property1 = left as PropertyInfo) != null && (property2 = right as PropertyInfo) != null)
			return property1 == property2;

		return false;
	}

	public static bool operator !=(MemberInfo left, MemberInfo right)
	{
		return !(left == right);
	}
	#endif // !FEATURE_CORECLR

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	#if !FEATURE_CORECLR
	// this method is required so Object.GetType is not made finalby the compiler
	Type _MemberInfo.GetType()
	{ 
		return base.GetType();
	}

	void _MemberInfo.GetTypeInfoCount(out uint pcTInfo)
	{
		throw new NotImplementedException();
	}

	void _MemberInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
	{
		throw new NotImplementedException();
	}

	void _MemberInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
	{
		throw new NotImplementedException();
	}

	void _MemberInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
	{
		throw new NotImplementedException();
	}
	#endif*/
}


struct BindingFlags// Enum
{
	public int __Value;
	alias __Value this;
	public enum __IsEnum = true; // Identifies struct as enum
	public this(int value)
	{
		__Value = value;
	}

	public Type GetType()
	{
		return __TypeOf!(typeof(this));
	}
	public enum BindingFlags Default = 0x00;
	public enum BindingFlags IgnoreCase = 0x01;
	public enum BindingFlags DeclaredOnly = 0x02;
	public enum BindingFlags Instance = 0x04;
	public enum BindingFlags Static = 0x08;
	public enum BindingFlags Public = 0x10;
	public enum BindingFlags NonPublic = 0x20;
	public enum BindingFlags FlattenHierarchy = 0x40;
	public enum BindingFlags InvokeMethod = 0x0100;
	public enum BindingFlags CreateInstance = 0x0200;
	public enum BindingFlags GetField = 0x0400;
	public enum BindingFlags SetField = 0x0800;
	public enum BindingFlags GetProperty = 0x1000;
	public enum BindingFlags SetProperty = 0x2000;
	public enum BindingFlags PutDispProperty = 0x4000;
	public enum BindingFlags PutRefDispProperty = 0x8000;
	public enum BindingFlags ExactBinding = 0x010000;
	public enum BindingFlags SuppressChangeType = 0x020000;
	public enum BindingFlags OptionalParamBinding = 0x040000;
	public enum BindingFlags IgnoreReturn = 0x01000000;


	BindingFlags opBinary(string op)(BindingFlags rhs)
	{
		return mixin("BindingFlags(__Value "~op~" rhs.__Value)");
	}
	bool opEquals(const BindingFlags a)
	{
		return a.__Value == this.__Value;
	}
	bool opEquals(const int a)
	{
		return a == this.__Value;
	}
	public string toString()
	{
		if (this == Default.__Value)
		{
            return "Default";
		}
		if (this == IgnoreCase.__Value)
		{
            return "IgnoreCase";
		}
		if (this == DeclaredOnly.__Value)
		{
            return "DeclaredOnly";
		}
		if (this == Instance.__Value)
		{
            return "Instance";
		}
		if (this == Static.__Value)
		{
            return "Static";
		}
		if (this == Public.__Value)
		{
            return "Public";
		}
		if (this == NonPublic.__Value)
		{
            return "NonPublic";
		}
		if (this == FlattenHierarchy.__Value)
		{
            return "FlattenHierarchy";
		}
		if (this == InvokeMethod.__Value)
		{
            return "InvokeMethod";
		}
		if (this == CreateInstance.__Value)
		{
            return "CreateInstance";
		}
		if (this == GetField.__Value)
		{
            return "GetField";
		}
		if (this == SetField.__Value)
		{
            return "SetField";
		}
		if (this == GetProperty.__Value)
		{
            return "GetProperty";
		}
		if (this == SetProperty.__Value)
		{
            return "SetProperty";
		}
		if (this == PutDispProperty.__Value)
		{
            return "PutDispProperty";
		}
		if (this == PutRefDispProperty.__Value)
		{
            return "PutRefDispProperty";
		}
		if (this == ExactBinding.__Value)
		{
            return "ExactBinding";
		}
		if (this == SuppressChangeType.__Value)
		{
            return "SuppressChangeType";
		}
		if (this == OptionalParamBinding.__Value)
		{
            return "OptionalParamBinding";
		}
		if (this == IgnoreReturn.__Value)
		{
            return "IgnoreReturn";
		}
		return std.conv.to!string(GetType().FullName);
	}
}
alias __Delegate!(bool delegate(Type m, NObject filterCriteria)) TypeFilter;

//alias  System.__Internal.Reflection.ValueMetadata ValueMetadata;

public class FieldInfo: MemberInfo
{

	public FieldAttributes Attributes;
	public Type FieldType;

	public override MemberTypes MemberType()@property { 
		return MemberTypes.Field; 
	}
	abstract void SetValue(NObject instance,NObject value);

	abstract NObject GetValue(NObject instance);

	public bool IsPublic() {  { return(Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public; } }

	public bool IsPrivate() {  { return(Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Private; } }

	public bool IsFamily() {  { return(Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Family; } }

	public bool IsAssembly() {  { return(Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Assembly; } }

	public bool IsFamilyAndAssembly() {  { return(Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.FamANDAssem; } }

	public bool IsFamilyOrAssembly() {  { return(Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.FamORAssem; } }

	public bool IsStatic() {  { return(Attributes & FieldAttributes.Static) != 0; } }

	public bool IsInitOnly() {  { return(Attributes & FieldAttributes.InitOnly) != 0; } }

	public bool IsLiteral() {  { return(Attributes & FieldAttributes.Literal) != 0; } }

	public bool IsNotSerialized() {  { return(Attributes & FieldAttributes.NotSerialized) != 0; } }

	public bool IsSpecialName()  {  { return(Attributes & FieldAttributes.SpecialName) != 0; } }

	public bool IsPinvokeImpl() {  { return(Attributes & FieldAttributes.PinvokeImpl) != 0; } }
}

public class FieldInfo__G(C,T) : FieldInfo
{
	alias T          return_type;
	alias return_type function(C*) getter_type;
	alias void function(C*,T) setter_type;

	getter_type _getter;
	setter_type _setter;

	override NObject[] GetCustomAttributes(bool inherit)
	{return null;}
	override NObject[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		return null;
	}
	override bool IsDefined(Type attributeType, bool inherit)
	{
		return true;
	}

	this(getter_type getter, setter_type setter)
	{
		_getter = getter;
		_setter = setter;
	}

	override void SetValue(NObject instance, NObject value)
	{

	//	Console.WriteLine(String.Format(_S("Setting {0} to {1}"), [value, instance]));
		if(instance is null)
		{
			//Console.WriteLine("instance is null");

		//	if(cast(Boxed!T)value)
			{
				_setter(null,UNBOX!(T)(value));
				return;
			}
		}
		else
		{
		//if(cast(Boxed!T)value)
		{
			//Console.WriteLine("instance is not null");

			_setter((UNBOX_R!(C)(instance)),UNBOX!(T)(value));
			return;
		}
		}

		//_setter(cast(C)instance, value);

	}

	override NObject GetValue(NObject instance)
	{
		if(instance is null) // Should actually check if this is a static field instead of this
		{
			return  BOX!(T)(_getter(null));

		}
		else
		{
			return  BOX!(T)(_getter((UNBOX_R!(C)(instance))));
		}
		//return null;
		//


	}

}


class PropertyInfo : MemberInfo
{
	public Type PropertyType;

	public override  MemberTypes MemberType() @property 
	{ 
		return MemberTypes.Property; 
	}

	abstract void SetValue(NObject instance,NObject value,NObject[] index =null);

	abstract NObject GetValue(NObject instance,NObject[] index = null);

	public abstract MethodInfo[] GetAccessors(bool nonPublic);

	public abstract MethodInfo GetGetMethod(bool nonPublic);

	public abstract MethodInfo GetSetMethod(bool nonPublic);

	public abstract ParameterInfo[] GetIndexParameters();

	//public abstract PropertyAttributes Attributes();// { get; }

	public  bool CanRead;// { get; }

	public  bool CanWrite;// { get; }

}

class PropertyInfo__G(C,T) : PropertyInfo
{

	alias T          return_type;
	alias return_type function(C*) getter_type;
	alias void function(C*,T) setter_type;

	getter_type _getter;
	setter_type _setter;


	NObject[] GetCustomAttributes(bool inherit){
	return null;
	}
		NObject[] GetCustomAttributes(Type attributeType, bool inherit){return null;}
			bool IsDefined(Type attributeType, bool inherit){return false;}
				MethodInfo[] GetAccessors(bool nonPublic){return null;}
					MethodInfo GetGetMethod(bool nonPublic){return null;}
						MethodInfo GetSetMethod(bool nonPublic){return null;}
							ParameterInfo[] GetIndexParameters(){return null;}
							
	this(getter_type getter, setter_type setter)
	{
		_getter = getter;
		_setter = setter;
	}

	override void SetValue(NObject instance, NObject value, NObject[] index= null)
	{
		//if(cast(Boxed!T)value)
		{
			_setter(UNBOX_R!(C)(instance),UNBOX!(T)(value));
			return;
		}

		//_setter(cast(C)instance, value);

	}

	override NObject GetValue(NObject instance, NObject[] index = null) // For indexed properties like string.Chars etc ...
	{
		return BOX!(T)(_getter(UNBOX_R!(C)(instance)));
	}

}

public class MethodBase : MemberInfo
{
	 ParameterInfo[] GetParametersNoCopy() { 
		 return GetParameters (); 
	 }

	//[System.Diagnostics.Contracts.Pure]
        public  ParameterInfo[] GetParameters()
		{
			return null;
		}

	public  MethodImplAttributes MethodImplementationFlags()  @property
	{
		
			return GetMethodImplementationFlags();
		
	}

	public  MethodImplAttributes GetMethodImplementationFlags()
	{
		return  MethodImplAttributes.init;
	}

//public abstract RuntimeMethodHandle MethodHandle() @property;

	public abstract MethodAttributes Attributes () @property;

	public abstract NObject Invoke(NObject obj, BindingFlags invokeAttr, Binder binder, NObject[] parameters, CultureInfo culture);

	public  CallingConventions CallingConvention() {  return CallingConventions.Standard; }

	//[System.Runtime.InteropServices.ComVisible(true)]
        public Type[] GetGenericArguments() { 
			return null;
		//	throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride")); 
		}

	public bool IsGenericMethodDefinition() {  return false;  }

	public bool ContainsGenericParameters() { return false;  }

	public bool IsGenericMethod() {  return false;  }

	public bool IsSecurityCritical() {  throw new NotImplementedException(); }

	public bool IsSecuritySafeCritical() {  throw new NotImplementedException();  }

	public bool IsSecurityTransparent() {  throw new NotImplementedException();  }

	
        public NObject Invoke(NObject obj, NObject[] parameters)
        {
            // Theoretically we should set up a LookForMyCaller stack mark here and pass that along.
            // But to maintain backward compatibility we can't switch to calling an 
            // internal overload that takes a stack mark.
            // Fortunately the stack walker skips all the reflection invocation frames including this one.
            // So this method will never be returned by the stack walker as the caller.
            // See SystemDomain::CallersMethodCallbackWithStackMark in AppDomain.cpp.
            return Invoke(obj, BindingFlags.Default, null, parameters, null);
        }

	public bool IsPublic()  {  { return(Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public; } }

	public bool IsPrivate() {  { return(Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private; } }

	public bool IsFamily() {  { return(Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Family; } }

	public bool IsAssembly() {  { return(Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Assembly; } }

	public bool IsFamilyAndAssembly() {  { return(Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamANDAssem; } }

	public bool IsFamilyOrAssembly() {  {return(Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamORAssem; } }

	public bool IsStatic() {  { return(Attributes & MethodAttributes.Static) != 0; } }

	public bool IsFinal() {  { return(Attributes & MethodAttributes.Final) != 0; }
	}
	public bool IsVirtual() {  { return(Attributes & MethodAttributes.Virtual) != 0; }
	}   
	public bool IsHideBySig() {  { return(Attributes & MethodAttributes.HideBySig) != 0; } }  

	public bool IsAbstract() {  { return(Attributes & MethodAttributes.Abstract) != 0; } }

	public bool IsSpecialName() {  { return(Attributes & MethodAttributes.SpecialName) != 0; } }

	public bool IsConstructor() 
        {
            return false;
         /*   {
                // To be backward compatible we only return true for instance RTSpecialName ctors.
                return (this is ConstructorInfo &&
                        !IsStatic &&
					((Attributes & MethodAttributes.RTSpecialName) == MethodAttributes.RTSpecialName));
            }*/
        }

	
    /*    public MethodBody GetMethodBody()
        {
            throw new InvalidOperationException();
        }   */     
	
	// helper method to construct the string representation of the parameter list

	 static string ConstructParameters(Type[] parameterTypes, CallingConventions callingConvention, bool serialization)
	{
		/*StringBuilder sbParamList = new StringBuilder();
		string comma = "";

		for (int i = 0; i < parameterTypes.Length; i++)
		{
			Type t = parameterTypes[i];

			sbParamList.Append(comma);

			string typeName = t.FormatTypeName(serialization);

			// Legacy: Why use "ByRef" for by ref parameters? What language is this? 
			// VB uses "ByRef" but it should precede (not follow) the parameter name.
			// Why don't we just use "&"?
			if (t.IsByRef && !serialization)
			{
				sbParamList.Append(typeName.TrimEnd([ '&' ]));
				sbParamList.Append(" ByRef");
			}
			else
			{
				sbParamList.Append(typeName);
			}

			comma = ", ";
		}

		if ((callingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs)
		{
			sbParamList.Append(comma);
			sbParamList.Append("...");
		}

		return sbParamList.ToString();*/
		return null;
	}

	/* string FullName()
	{
		//get
		{
			return String.Format("{0}.{1}", DeclaringType.FullName, FormatNameAndSig());
		}
	}

	 string FormatNameAndSig()
	{
		return FormatNameAndSig(false);
	}*/

	/*string FormatNameAndSig(bool serialization)
	{
		// Serialization uses ToString to resolve MethodInfo overloads.
		StringBuilder sbName = new StringBuilder(Name);

		sbName.Append("(");
		sbName.Append(ConstructParameters(GetParameterTypes(), CallingConvention, serialization));
		sbName.Append(")");

		return sbName.ToString();
	}*/

	/*internalType[] GetParameterTypes()
	{
		ParameterInfo[] paramInfo = GetParametersNoCopy();

		Type[] parameterTypes = new Type[paramInfo.Length];
		for (int i = 0; i < paramInfo.Length; i++)
			parameterTypes[i] = paramInfo[i].ParameterType;

		return parameterTypes;
	}*/

	//[System.Security.SecuritySafeCritical]
      NObject[] CheckArguments(NObject[] parameters, Binder binder, 
										 BindingFlags invokeAttr, CultureInfo culture, Signature sig)
	{
		/*// copy the arguments in a different array so we detach from any user changes 
		Object[] copyOfParameters = new Object[parameters.Length];

		ParameterInfo[] p = null;
		for (int i = 0; i < parameters.Length; i++)
		{
			Object arg = parameters[i];
			RuntimeType argRT = sig.Arguments[i];

			if (arg == Type.Missing)
			{
				if (p == null) 
					p = GetParametersNoCopy();
				if (p[i].DefaultValue == System.DBNull.Value)
					throw new ArgumentException(Environment.GetResourceString("Arg_VarMissNull"),"parameters");
				arg = p[i].DefaultValue;
			}
			copyOfParameters[i] = argRT.CheckValue(arg, binder, culture, invokeAttr);
		}

		return copyOfParameters;*/
		return null;

	}
}

public class MethodInfo: MethodBase
{
	Type[] Params; //Temporary measure

	public  override MemberTypes MemberType()@property {  
		return MemberTypes.Method;
	}
	

	public  Type ReturnType;// { get { throw new NotImplementedException(); } }

	public  ParameterInfo ReturnParameter;
	//{ get { throw new NotImplementedException(); } }

//public abstract ICustomAttributeProvider ReturnTypeCustomAttributes;// { get;  }

	public abstract MethodInfo GetBaseDefinition();

        public override Type[] GetGenericArguments(){return null;};// { throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride")); }

	//[System.Runtime.InteropServices.ComVisible(true)]
        public MethodInfo GetGenericMethodDefinition()
		{return null;
		}
		//{ throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride")); }

	public  MethodInfo MakeGenericMethod(Type[] typeArguments...) 
	{ 
	//	throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride")); 
		return null;

	}

	public  Delegate CreateDelegate(Type delegateType) {
		return null;
	}

	public  Delegate CreateDelegate(Type delegateType, NObject target) { 
		return null;

	}


	NObject Invoke(NObject object, Array_T!(NObject) parameters)
	{
		return null;
	}
}

class MethodInfo__G(T...) : MethodInfo
{
	

	alias std.traits.ReturnType!(T[1])          return_type;
	alias ParameterTypeTuple!(T[1])  param_types;
	alias return_type function(param_types)  method_type;
	alias return_type delegate(param_types)  delegate_type;
	method_type _method;
	//delegate_type _delegate;

NObject[] GetCustomAttributes(bool inherit) {
return null;
}
	NObject[] GetCustomAttributes(Type attributeType, bool inherit){
	return null;
	}
		bool IsDefined(Type attributeType, bool inherit){
		return false;
		}
			MethodAttributes Attributes() @property{
			return MethodAttributes.init;
			}
				NObject Invoke(NObject obj, BindingFlags invokeAttr, Binder binder, NObject[] parameters, CultureInfo culture)
				{
				return null;
				}
					MethodInfo GetBaseDefinition(){
					return null;
					}


	this(method_type method)
	{
		_method = method;

		foreach(i, ty; param_types)
		{
			Params ~= __TypeOf!(param_types[i]);
		}


	}

	/*this(delegate_type method)
	{
	_delegate = method;
	}*/

	/*override Object Invoke(null_t, Object[] _arguments)
	{
		return _invoke(null, _arguments);
	}

	override Object Invoke(void* pod, Object[] _arguments) 
	{
		/*if(pod)
		{
		static if(__traits(isStaticFunction, T[1]))
		throw new Exception("Invoke Failed: Struct Pointer Provided for Static Method");
		}
		else
		{
		static if(!__traits(isStaticFunction, T[1]))
		throw new Exception("Invoke Failed: Struct Pointer is Null");
		}*/

		/*return _invoke(&pod, _arguments);
	}*/

	override NObject Invoke(NObject obj, Array_T!(NObject) _arguments)
	{
		if(obj)
		{
			/*static if(__traits(isStaticFunction, T[1]))
			{
			throw new Exception("Invoke Failed: Object Reference Provided for Static Method");
			}
			else
			{
			if(typeid(obj) != typeid(T[0]))
			{
			throw new Exception("Invoke Failed: Invalid Object Type - Expected " 
			~ T[0].stringof ~ " Got " ~ typeid(obj).toString());
			}
			}*/
		}
		else
		{
			/*static if(!__traits(isStaticFunction, T[1]))
			throw new Exception("Invoke Failed: Object Reference is Null");*/
		}

		if(_arguments !is null)
			return _invoke(obj, _arguments.Items);
		else 
			return  _invoke(obj, []);
	}

	NObject _invoke(NObject ptr,  NObject[] _arguments) 
		//	Variant _invoke(void *ptr, ref va_list _argptr, TypeInfo[] _arguments) Crashes on OSX dont know why _arguments becomes null
	{

		//std.stdio.writeln(_arguments.length);
		//std.stdio.writeln(param_types.length);
		//std.stdio.writeln(is(typeof(ptr)==T[0]));

		//std.stdio.writeln(cast(Boxed!(T[0])) ptr);
//!(cast(T[0]) ptr) && 
		/*if(!(cast(Boxed!(T[0])) ptr))
		{
			throw new Exception("System.Reflection.TargetException: Object does not match target type");
		}*/

		// S


		if(_arguments.length != param_types.length)
		{
			//if(param_types.length !=0)
				throw new Exception("TargetParameterCountException: Parameter count mismatch");
				//throw new Exception("Invoke Failed: Invalid Argument Count: " ~ _arguments.length.stringof ~":" ~ param_types.length.stringof);
		}


		param_types args;

		foreach(i, ty; param_types)
		{
			alias type = param_types[i];
			auto arg = cast(param_types[i])cast(void*)(_arguments[i]);


			if(cast(Boxed!(type))_arguments[i])
			{
				//Boxed Value, this is Ok

				args[i] = ((cast(Boxed!(type))cast(void*)_arguments[i])).__Value;

			}
			else {

				if((typeid(_arguments[i]) != typeid(param_types[i])) && (typeid(_arguments[i]) != typeid(Boxed!(param_types[i]))) )// Test for boxed
				{

					throw new Exception("Invoke Failed: Invalid Argument Type - " ~
										"Expected \'" ~ param_types[i].stringof ~ "\', " ~
										"Received \'" ~ _arguments[i].toString ~ "\'.");
				}
				args[i] = arg;
			}

		}

		if(ptr) // TODO: change this to !isStatic
		{
			delegate_type del;

			//if(_method !is null)
			del.funcptr =_method;//addressOf!(T[1]);

			del.ptr = cast(void *)UNBOX!(T[0])(ptr);

			static if(is(return_type == void))
			{
				del(args);
				return null;
			}
			else
			{
				static if(is(return_type:Object))
					return del(args);
				else
					return  BOX!(return_type)(del(args));
			}
		}
		else
		{
			//	if(_method !is null)
			method_type del = _method;//addressOf!(T[1]);
			//del.ptr = ptr;

			static if(is(return_type == void))
			{
				del(args);
				return null;
			}
			else
			{
				static if(is(return_type:Object))
					return del(args);
				else
					return BOX!(return_type)(del(args));
			}
		}
		return null;
	}


}

public class ConstructorInfo: MethodBase
{

	public  override MemberTypes MemberType()@property {  
		return MemberTypes.Method;
	}


	public  Type ReturnType;// { get { throw new NotImplementedException(); } }

	public  ParameterInfo ReturnParameter;
	//{ get { throw new NotImplementedException(); } }

	//public abstract ICustomAttributeProvider ReturnTypeCustomAttributes;// { get;  }

	public abstract MethodInfo GetBaseDefinition();

	public override Type[] GetGenericArguments(){return null;};// { throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride")); }

	//[System.Runtime.InteropServices.ComVisible(true)]
	public MethodInfo GetGenericMethodDefinition()
	{return null;
	}
	//{ throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride")); }

	public  MethodInfo MakeGenericMethod(Type[] typeArguments...) 
	{ 
		//	throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride")); 
		return null;

	}

	public  Delegate CreateDelegate(Type delegateType) {
		return null;
	}

	public  Delegate CreateDelegate(Type delegateType, NObject target) { 
		return null;

	}


	NObject Invoke(NObject object, Array_T!(NObject) parameters)
	{
		return null;
	}
}

class ConstructorInfo__G(T...) : ConstructorInfo
{
	Type[] Params;

	alias std.traits.ReturnType!(T[1])          return_type;
	alias ParameterTypeTuple!(T[1])  param_types;
	alias return_type function(param_types)  method_type;
	alias return_type delegate(param_types)  delegate_type;
	method_type _method;
	//delegate_type _delegate;

	NObject[] GetCustomAttributes(bool inherit) {
		return null;
	}
	NObject[] GetCustomAttributes(Type attributeType, bool inherit){
		return null;
	}
	bool IsDefined(Type attributeType, bool inherit){
		return false;
	}
	MethodAttributes Attributes() @property{
		return MethodAttributes.init;
	}
	NObject Invoke(NObject obj, BindingFlags invokeAttr, Binder binder, NObject[] parameters, CultureInfo culture){
		return null;
	}
	MethodInfo GetBaseDefinition(){
		return null;
	}


	this(method_type method)
	{
		_method = method;

		foreach(i, ty; param_types)
		{
			Params ~= __TypeOf!(param_types[i]);
		}


	}

	/*this(delegate_type method)
	{
	_delegate = method;
	}*/

	/*override Object Invoke(null_t, Object[] _arguments)
	{
	return _invoke(null, _arguments);
	}

	override Object Invoke(void* pod, Object[] _arguments) 
	{
	/*if(pod)
	{
	static if(__traits(isStaticFunction, T[1]))
	throw new Exception("Invoke Failed: Struct Pointer Provided for Static Method");
	}
	else
	{
	static if(!__traits(isStaticFunction, T[1]))
	throw new Exception("Invoke Failed: Struct Pointer is Null");
	}*/

	/*return _invoke(&pod, _arguments);
	}*/

	override NObject Invoke(NObject obj, Array_T!(NObject) _arguments)
	{
		if(obj)
		{
			/*static if(__traits(isStaticFunction, T[1]))
			{
			throw new Exception("Invoke Failed: Object Reference Provided for Static Method");
			}
			else
			{
			if(typeid(obj) != typeid(T[0]))
			{
			throw new Exception("Invoke Failed: Invalid Object Type - Expected " 
			~ T[0].stringof ~ " Got " ~ typeid(obj).toString());
			}
			}*/
		}
		else
		{
			/*static if(!__traits(isStaticFunction, T[1]))
			throw new Exception("Invoke Failed: Object Reference is Null");*/
		}

		return _invoke(cast(void*)obj, _arguments.Items);
	}

	NObject _invoke(void *ptr,  NObject[] _arguments) 
		//	Variant _invoke(void *ptr, ref va_list _argptr, TypeInfo[] _arguments) Crashes on OSX dont know why _arguments becomes null
	{

		//std.stdio.writeln(_arguments.length);

		if(_arguments.length != param_types.length)
		{
			if(param_types.length !=0)
				throw new Exception("Invoke Failed: Invalid Argument Count: " ~ _arguments.length.stringof ~":" ~ param_types.length.stringof);
		}


		param_types args;

		foreach(i, ty; param_types)
		{
			alias type = param_types[i];
			auto arg = cast(param_types[i])cast(void*)(_arguments[i]);


			if(cast(Boxed!(type))_arguments[i])
			{
				//Boxed Value, this is Ok

				args[i] = ((cast(Boxed!(type))cast(void*)_arguments[i])).__Value;

			}
			else {

				if((typeid(_arguments[i]) != typeid(param_types[i])) && (typeid(_arguments[i]) != typeid(Boxed!(param_types[i]))) )// Test for boxed
				{

					throw new Exception("Invoke Failed: Invalid Argument Type - " ~
										"Expected \'" ~ param_types[i].stringof ~ "\', " ~
										"Received \'" ~ _arguments[i].toString ~ "\'.");
				}
				args[i] = arg;
			}

		}

		if(ptr) // TODO: change this to !isStatic
		{
			delegate_type del;

			//if(_method !is null)
			del.funcptr =_method;//addressOf!(T[1]);

			del.ptr = ptr;

			static if(is(return_type == void))
			{
				del(args);
				return null;
			}
			else
			{
				static if(is(return_type:Object))
					return del(args);
				else
					return  BOX!(return_type)(del(args));
			}
		}
		else
		{
			//	if(_method !is null)
			method_type del = _method;//addressOf!(T[1]);
			//del.ptr = ptr;

			static if(is(return_type == void))
			{
				del(args);
				return null;
			}
			else
			{
				static if(is(return_type:Object))
					return del(args);
				else
					return BOX!(return_type)(del(args));
			}
		}
		return null;
	}


}

public class Type_T(T):Type
{



	alias System.Namespace.Type SType;

	override Array_T!(SType) GetInterfaces()
	{
		Array_T!(SType) interfaces = null;

		static if(InterfacesTuple!(T).length > 0)
		{
			SType[] interfaces_;


			alias rinterfaces = InterfacesTuple!(T);
			foreach (i, T; rinterfaces)
			{
				interfaces_ ~= __TypeOf!(T);
				//				std.stdio.write(interfaces_);
				//std.stdio.writefln("TL[%d] = %s", i, typeid(T));
			}

			interfaces = __ARRAY!(SType)(interfaces_);
		}

		return interfaces;
	}

	override Array_T!(SType)  FindInterfaces(TypeFilter filter,
	                                         NObject filterCriteria)
	{
		//Should filter the interfaces out using filter
		return GetInterfaces();

	}

	static if(!is(T==void))
	{

		//T Type = __Default!(T);
	}
	else
	{
		__Void Type = __Void();
	}

	static if (!is(T:NException) && !__isPointer!(T))
	{
		public override NObject Create()
		{

			static if(!__isInterface!(T))
			{
		//	Console.WriteLine("Creating instance of "  ~ T.stringof);

			auto newType=__TypeNew!(T)();

			//if(is(T==class))
			//	return newType;
			//Console.WriteLine("Created instance of "  ~ typeof(newType).stringof);

			/*static if(__isScalar!(T) || __isStruct!(T) || __isEnum!(T) || __isArray!(T))
			{
			auto boxed= BOX!(T)(newType);

			//Console.WriteLine("returning object of "  ~ typeof(boxed).stringof);

			return boxed;
			}*/
			
			return BOX!(T)(newType);
			}
			else
				return null;
		}
	}
	else
	{
		public override NObject Create()
		{
			return null; // TODO: Make it possible to create an instance of an Exception			
		}
	}

	

	this(string csName=null)
	{


		/*	Type = type;

		//Generating reflection info slows down compile considerably ... will use roslyn to generate this info, its much faster
		*/
		version(Reflection)
		{
			__Meta = createMetadata!(T);
		}

		

		if(is(T==class) || is(T==struct) || is(T==interface))
		{


			static if(is(T==class))
			{

				static if((BaseClassesTuple!T).length > 0)
				{
					//if((typeid(T) != typeid(NObject)) && (typeid(T) != typeid(Object))) 
					static if(is(T:NObject))
					{
						static if(!is(T==NObject)&&!is(T==NException))
						{
							BaseType = __TypeOf!(BaseClassesTuple!T[0]);
						}
						else
						{
							BaseType = null;
						}

						static if(is(T==NException))
						{
							BaseType = __TypeOf!(NObject);
						}
					}
				}
			}


			auto name = T.stringof;//System.Namespace.Boxed!(

			auto fullName = fullyQualifiedName!(T);

			fullName = fullName.replace("CsRoot.",""); //Make sure to mark this namespace as global

			//	Console.WriteLine("name " ~ name);
			//Console.WriteLine("fullName "~fullName);

			if(name.lastIndexOf("Boxed!(")!=-1)
			{
				name = name[("Boxed!(").length..$-1];
			}

			if(fullName.lastIndexOf("System.Namespace.Boxed!(")!=-1)
			{
				fullName = fullName[("System.Namespace.Boxed!(").length..$-1];
			}

			//	Console.WriteLine("name " ~ name);
			//Console.WriteLine("fullName "~fullName);

			auto dotname = "." ~ name;
			if(fullName.lastIndexOf(dotname)!=-1)
			{
				//Console.WriteLine(fullName ~ "contains" ~ dotname);
				fullName = (fullName[0..fullName.lastIndexOf(dotname)]);
			}



			FullName = _S(fullName);



			Name = _S(name);
		}
		else
		{
			
			if(is(T==void))
			{
				FullName = _S("System.Void");
				Name = _S("Void");
			}
			else
			{
		/*	auto boxedType = __GetBoxedType!(T);
			auto fullName = (boxedType.FullName);
			FullName=boxedType.FullName;
			Name = boxedType.Name;*/
			}
		}

		//enum templateParams = TemplateArgsOf!(T);

		if(csName!=null)
		{
			FullName = _S(csName);
			if(csName.lastIndexOf(".")!=-1)
			{
				Name = _S(csName[csName.lastIndexOf(".")..$]);
			}
		}

		//FullName =_S("Dummy");
		//	Console.WriteLine(FullName);
		//		Console.WriteLine(Name);

		static if(is(T==interface))
		{
			//			Console.WriteLine("this is an interface");
			IsInterface = true;
		}

		//members
		/*static if(is(T==interface)||is(T==class)||is(T==struct))
		{
			enum allMembers = __traits(allMembers, T);
			foreach(member; allMembers) // compile time
			{
				__members ~= member;
				__memberPointers~=cast(void*) &__traits(getMember,T,member);
			}
			
		}


		
//runtime
		Console.WriteLine(_S("Members Of :") + FullName);
		int count = 0;
		foreach(member; __members) 
		{

			Console.WriteLine(member);
			Console.WriteLine(__memberPointers[count]);

		}*/

	}

	public R GetMember(R)(string name)
	{
		return cast(R) [__traits(getMember, T, name)][0];
	}


	/*static if(!is(T==void))
	{
	public override NObject __GetValue(String fieldname, NObject underlyingtype=null)
	{
		/*T anobject;

		static if(is(T==class))
		{
			anobject  = underlyingtype is null?Type:cast(T)underlyingtype;
		}
		else  if(is(T==struct))
		{
			anobject  = underlyingtype is null?Type:UNBOX!(T)(underlyingtype);
		}

		//return __GetValue!(fieldname);
		//return _S("yo");
		//auto value = getValue(__Meta,cast(string)fieldname, anobject);

		//std.stdio.writeln(value);
		/*	std.stdio.writeln(anobject);
		string sfieldname = cast(string)fieldname;
		std.stdio.writeln(sfieldname);
		__Meta.getValue(sfieldname, anobject);
		std.stdio.writeln("attempting to get value of " ~ sfieldname);

		return _S("nada");*/


		/*if(value.type==typeid(int))
		return BOX!int(value.get!int());
		if(value.type==typeid(const(int)))
		return BOX!int(value.get!(const(int))());
		else*/
		//return  value.get!NObject();
		/*return null;

	}*/


	

	/*public override Array_T!(String)  GetMembers(string name="")
	{
		static if(is(T==class) || is(T==struct) || is(T==interface))
		{
			if(name=="")
			{
					import std.algorithm;
				import std.array:array;
				auto membernames = new Array_T!(String)
				(
				__IA!(String[])(
				`	 ([__traits(allMembers,T)]
				.map!(a=>new String(a))).array
				)
				);

				return membernames;
			}
		}
		return null;
	}
	}*/

	override string toString()
	{
		return cast(string)FullName;
	}

	public  override String ToString()
	{
		return FullName;
	}
}