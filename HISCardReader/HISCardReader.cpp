// HISCardReader.cpp : 定義 DLL 應用程式的匯出函式。
//

#include "stdafx.h"

#include <windows.h>
#include <initguid.h>
#include <devguid.h>
#include <setupapi.h>

#include "HISCardReader.h"
 

vector<string>* NHISCardReader::CHISCardReader::GetComportDeviceNameList()
{
	vector<string>* deviceList = new vector<string>();
	SP_DEVINFO_DATA devInfoData = {};
	devInfoData.cbSize = sizeof(devInfoData);

	// get the tree containing the info for the ports
	HDEVINFO hDeviceInfo = SetupDiGetClassDevs(&GUID_DEVCLASS_PORTS,
		0,
		nullptr,
		DIGCF_PRESENT
	);
	if (hDeviceInfo == INVALID_HANDLE_VALUE)
	{
		return;
	}

	// iterate over all the devices in the tree
	int nDevice = 0;
	while (SetupDiEnumDeviceInfo(hDeviceInfo,            // Our device tree
		nDevice++,            // The member to look for
		&devInfoData))
	{
		DWORD regDataType;
		DWORD reqSize = 0;

		// find the size required to hold the device info
		SetupDiGetDeviceRegistryProperty(hDeviceInfo, &devInfoData, SPDRP_HARDWAREID, nullptr, nullptr, 0, &reqSize);
		BYTE* hardwareId = new BYTE[(reqSize > 1) ? reqSize : 1];
		// now store it in a buffer
		if (SetupDiGetDeviceRegistryProperty(hDeviceInfo, &devInfoData, SPDRP_HARDWAREID, &regDataType, hardwareId, sizeof(hardwareId) * reqSize, nullptr))
		{
			// find the size required to hold the friendly name
			reqSize = 0;
			SetupDiGetDeviceRegistryProperty(hDeviceInfo, &devInfoData, SPDRP_FRIENDLYNAME, nullptr, nullptr, 0, &reqSize);
			BYTE* friendlyName = new BYTE[(reqSize > 1) ? reqSize : 1];
			// now store it in a buffer
			if (!SetupDiGetDeviceRegistryProperty(hDeviceInfo, &devInfoData, SPDRP_FRIENDLYNAME, nullptr, friendlyName, sizeof(friendlyName) * reqSize, nullptr))
			{
				// device does not have this property set
				memset(friendlyName, 0, reqSize > 1 ? reqSize : 1);
			}
			// use friendlyName here

			string str = "";
			for (int i = 0; i < reqSize;i++)
			{
				str += *(friendlyName + i);
			}
			deviceList->push_back(str);
			delete[] friendlyName;
		}
		delete[] hardwareId;
		
		return deviceList;
	}
}

NHISCardReader::HisCardReader::HisCardReader()
{
	m_HISCardReader = new CHISCardReader();
}

System::Collections::Generic::List<System::String^>^ NHISCardReader::HisCardReader::GetComportDeviceNameList()
{
	vector<string>* deviceList = m_HISCardReader->GetComportDeviceNameList();

	
	
}
