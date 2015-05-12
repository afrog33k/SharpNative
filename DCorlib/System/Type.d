module System.Type;

//Basic System.Type, to support for enums for now
import System.Namespace;
import System.__Internal.Namespace;
import System.Reflection.Internal;
import System.Reflection.Namespace;


public class Type:NObject
{
	public String Name;
	public String FullName;
	public Type BaseType;
	Type[] Interfaces;
	public bool IsInterface;

	MethodInfo [] __methods;
	FieldInfo [] __fields;
	PropertyInfo [] __properties;
	ConstructorInfo[] __constructors;

	abstract Array_T!(Type)  FindInterfaces(TypeFilter filter,
	                                        NObject filterCriteria);

	abstract Array_T!(Type) GetInterfaces();

	public NObject Create()
	{
		return null;
	}


	public Array_T!(System.Reflection.Namespace.MemberInfo) GetMembers(string name="")
	{
		return new Array_T!(System.Reflection.Namespace.MemberInfo)(__CC!(System.Reflection.Namespace.MemberInfo[])(cast(System.Reflection.Namespace.MemberInfo[])__methods ~ cast(System.Reflection.Namespace.MemberInfo[])__fields ~ cast(System.Reflection.Namespace.MemberInfo[])__properties));
	}

	public Array_T!(MethodInfo) GetMethods()
	{
		if(BaseType !is null)
		{
			auto baseMethods = BaseType.GetMethods();

			MethodInfo[] matchingmethods = [];
			for(int i=0;i<baseMethods.Items.length;i++)
			{
				auto method = baseMethods.Items[i];
				if(!(method.Attributes & MethodAttributes.Virtual))
					matchingmethods ~= method;
			}

			return new Array_T!(MethodInfo)(__CC!(MethodInfo[])(__methods ~ matchingmethods));
		}
		
		return new Array_T!(MethodInfo)(__CC!(MethodInfo[])(__methods));
	}

	MethodAttributes __convertBindingFlags(BindingFlags bindingFlags)
	{
		MethodAttributes attr;

		if(bindingFlags & BindingFlags.Public)
			attr &= MethodAttributes.Public;

		if(bindingFlags & BindingFlags.Static)
			attr &= MethodAttributes.Static;

		return attr;
	}

	//BindingFlags bindingAttr
	public Array_T!(MethodInfo) GetMethods(BindingFlags bindingAttr)
	{
		auto methods =  GetMethods().Items;
		MethodInfo[] matchingmethods = [];
		auto attribs = __convertBindingFlags(bindingAttr);
		for(int i=0;i<methods.length;i++)
		{
			auto method = methods[i];
			if(method.Attributes & attribs)
				matchingmethods ~= method;
		}
		return new Array_T!(MethodInfo)(__CC!(MethodInfo[])(matchingmethods));
	}

	public Array_T!(FieldInfo) GetFields(String name=String.Empty)
	{
		return new Array_T!(FieldInfo)(__CC!(FieldInfo[])(__fields));
	}

	bool __SameParams(Type[] paramsA, Type[] paramsB)
	{
		if(paramsA is null && paramsB is null)
			return true;

		if(paramsA == paramsB)
			return true;

		if(paramsA.length != paramsB.length)
			return false;

		for(int c=0;c<paramsA.length;c++)
		{
			if(paramsA[c]!=paramsB[c])
				return false;
		}

		return true;
	}

	MethodInfo GetMethod(String name, Type[] params ...)
	{
		auto index = -1;

		for(int c =0; c < __methods.length; c++)
		{
			//Console.WriteLine("__methods[c].Params" ~ std.conv.to!string(__methods[c].Params.length));
			if(__methods[c].Name==name) 
			{
				if((params !is null)&&!__SameParams(__methods[c].Params, params))
				{
					
				}

				index = c;
				break;
			}

		}

		if(index==-1)
			return null;

		return __methods[index];
	}

	FieldInfo GetField(String name, BindingFlags flags=BindingFlags.Public)
	{
		auto index = -1;

		for(int c =0; c < __fields.length; c++)
		{
			if(__fields[c].Name==name)
			{
				index = c;
				break;
			}

		}

		if(index==-1)
			return null;

		return __fields[index];
	}

	PropertyInfo GetProperty(String name)
	{
		auto index = -1;

		for(int c =0; c < __properties.length; c++)
		{
			if(__properties[c].Name==name)
			{
				index = c;
				break;
			}

		}

		if(index==-1)
			return null;

		return __properties[index];
	}

	Type __Method(string name, MethodInfo info, MethodAttributes attributes = MethodAttributes.Public)
	{
		info.Name = _S(name);
		info.__rtAttributes = attributes;
		__methods ~= info;
		return this;
	}

	Type __Field(string name, FieldInfo info,  FieldAttributes attributes = FieldAttributes.Public)
	{
		info.Name = _S(name);
		info.Attributes = attributes;
		__fields ~= info;
		return this;
	}

	Type __Property(string name, PropertyInfo info)
	{
		info.Name = _S(name);
		__properties ~= info;
		return this;
	}

	Type __Constructor(string name, ConstructorInfo info)
	{
		info.Name = _S(name);
		__constructors ~= info;
		return this;
	}

	Type __Setup(Type baseClass, Type[] interfaces...)
	{
		BaseType = baseClass;
		Interfaces = interfaces;
		return this;
	}

	override bool Equals(NObject other)
	{
	
		auto otherType = cast(Type) other;

		if(otherType is null)
		{
			if(other !is null)
				otherType = other.GetType();
		}

		if(otherType is null)
			return false;

		auto isequal =  this.FullName.Hash == otherType.FullName.Hash;

		if(isequal)
			return true;


		{
			auto interfaces =this.GetInterfaces();
			if(interfaces !is null && interfaces.Length > 0)
			{
				for(int i=0;i<interfaces.Length;i++)
				{
					if(interfaces[i].FullName.Hash == otherType.FullName.Hash)
						return true;
				}
			}
		}


		if(this.IsInterface)
		{
			auto interfaces =otherType.GetInterfaces();
			if(interfaces !is null && interfaces.Length > 0)
			{
				for(int i=0;i<interfaces.Length;i++)
				{
					if(interfaces[i].FullName.Hash == this.FullName.Hash)
						return true;
				}
			}
		}


		return false;
	}

	public override String ToString()
	{
		return FullName;
	}
}