#pragma once
#include <string>
#include <vector>

using namespace std;

namespace NHISCardReader
{
	class CHISCardReader
	{
	public:
		vector<string>* GetComportDeviceNameList();
	};

	ref class HisCardReader
	{
	public:
		HisCardReader();
		
		System::Collections::Generic::List<System::String^>^ GetComportDeviceNameList();

	private:
		CHISCardReader* m_HISCardReader;
	};

}
