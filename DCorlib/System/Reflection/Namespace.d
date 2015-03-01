module System.Reflection.Namespace;
import System.Namespace;
import System.__Internal.Namespace;
import System.Reflection.Internal;
import std.traits;
import std.string;
import std.array;


alias __Delegate!(bool delegate(Type m, NObject filterCriteria)) TypeFilter;

//alias  System.__Internal.Reflection.ValueMetadata ValueMetadata;



public class FieldInfo: NObject
{
	Type DeclaringType;

	String Name;

	//ValueMetadata __Meta;

	NObject GetValue(NObject object=null)
	{
		//std.stdio.writeln("getting value for " ~ Name.Text);
		//return _S("failed");
		return DeclaringType.__GetValue(Name,object);
	}
}

public class MethodInfo: NObject
{
	Type DeclaringType;

	String Name;

	//ValueMetadata __Meta;

	NObject Invoke(NObject object, Array_T!(NObject) parameters)
	{
		//std.stdio.writeln("getting value for " ~ Name.Text);
		//return _S("failed");
		//return DeclaringType.__GetValue(Name,object);

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

	T Type = __Default!(T);
	}
	else
	{
		__Void Type = __Void();
	}

	static if (!is(T:NException))
	{
		public override NObject Create()
		{
		//	Console.WriteLine("Creating instance of "  ~ T.stringof);

			auto newType=__TypeNew!(T)();
			//Console.WriteLine("Created instance of "  ~ typeof(newType).stringof);

			auto boxed= BOX!(T)(newType);

			//Console.WriteLine("returning object of "  ~ typeof(boxed).stringof);

			return boxed;

		}
	}
	else
	{
		public override NObject Create()
		{
			return null; // TODO: Make it possible to create an instance of an Exception			
		}
	}

	public override Array_T!(String) GetMember(String name)
	{
		return null;
	}

	public override MethodInfo GetMethod(String name)
	{
		return null;//&__traits(getMember,T);
	}

	string[] __members;
	void*[] __memberPointers;


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
			auto boxedType = __GetBoxedType!(T);
			auto fullName = (boxedType.FullName);
			FullName=boxedType.FullName;
			Name = boxedType.Name;
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


	static if(!is(T==void))
	{
	public override NObject __GetValue(String fieldname, NObject underlyingtype=null)
	{
		T anobject;

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
		return null;

	}


	public override Array_T!FieldInfo GetFields(string name="")
	{
		FieldInfo result[];// = __Meta.Fields;

		//auto fields = __Meta.children;

		////Console.WriteLine(fields.values.length);

		//foreach(field; __Meta.Fields)
		//{
		//	auto fieldInfo = new FieldInfo();
		//	fieldInfo.Name = _S(field.symbol.name);
		//	fieldInfo.DeclaringType = this;
		//	result ~= fieldInfo;
		//}

		//Console.WriteLine(_S("In override ... "));

		//	auto rtInfo = typeid(T).rtInfo();

		// std.stdio.writeln(rtInfo);


		/*static if(is(T==class) || is(T==struct) || is(T==interface))
		{
		foreach (member_string; __traits(allMembers, T))
		{
		auto member =  T.stringof ~ "." ~ member_string;
		static if (!is(typeof(member) == function)/* && !is(typeof(member)==property))
		{
		auto fieldInfo = new FieldInfo();
		fieldInfo.Name = _S(member_string);
		fieldInfo.DeclaringType = this;
		result ~= fieldInfo;
		//	Console.WriteLine(_S(member_string));
		}
		}	
		}*/

		//return new Array_T!FieldInfo(__CC!(FieldInfo[])(result));

		return new Array_T!FieldInfo(__CC!(FieldInfo[])(result));
	}

	public override Array_T!(String)  GetMembers(string name="")
	{
		static if(is(T==class) || is(T==struct) || is(T==interface))
		{
			if(name=="")
			{
				/*	import std.algorithm;
				import std.array:array;
				auto membernames = new Array_T!(String)
				(
				__IA!(String[])(
				`	 ([__traits(allMembers,T)]
				.map!(a=>new String(a))).array
				)
				);

				return membernames;*/
			}
		}
		return null;
	}
	}

	override string toString()
	{
		return cast(string)FullName;
	}

	public  override String ToString()
	{
		return FullName;
	}
}