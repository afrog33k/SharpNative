module System.Enum;

import System.Namespace;

public class Enum : NObject
{
	public static Array_T!(String) GetNames(Type type)
	{
		auto members = type.GetMembers();
		String[] names;

		for(int c=0;c<members.Length;c++)
		{
			names ~= members[c].Name;
		}
		return  new Array_T!(String)(__CC(names));
	}

	public static long Parse(T)(Type_T!(T) type, String name)
	{
		enum allMembers = __traits(allMembers, T);

		if(type is null) 
			throw new ArgumentNullException(String("type"));
		if(name is null)
			throw new ArgumentNullException(String("name"));

		//auto temp = __TypeNew!(T);

		auto members =  type.GetMembers();
		auto mname = name.Text;

		foreach(member; members) 
		{
			if(member==name)
			{
				return type.GetMember!(long)(name);
				//return __traits(getMember,T,mname);
			}
		}


		/*for(int i = 0; i < members.Length; i++)
		{
		if(members[i]==name)
		{

		return __traits(getMember,temp,name.Text);
		}
		}*/

		throw new ArgumentException(String(""));
	}


}