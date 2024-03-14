// dllmain.cpp : 定义 DLL 应用程序的入口点。
//#include "pch.h"
#include <Windows.h>
#include <fstream>
#include <iostream>
#include <filesystem>
#include <string>

using namespace std;

namespace fs = std::filesystem;

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{

	if (ul_reason_for_call == DLL_PROCESS_ATTACH) {


		ofstream log;
		string line;
		ifstream str_bin(L".\\CE_DATA\\GECV_STR.bin", ios::in);
		log.open(".\\CE_DATA\\GECV_STR.log", std::ios::trunc | std::ios::out);

		log << "Start!\n";
		HANDLE process = OpenProcess(PROCESS_ALL_ACCESS, FALSE, GetCurrentProcessId());


		if (str_bin.is_open()) {
			log << "Load GECV_STR.bin\n";
			while (getline(str_bin, line)) {

				log << "Read Text:\"" << line << '\"' << '\n';

				string address = line.substr(0, 8);
				string data = line.substr(9);
				log << "Split Data:(16)Address:" << address << "&Data:" << data << '\n';

				int int_addr = stoi(address, 0, 16);

				SIZE_T char_length = data.length() + 1;

				char* char_arr = new char[char_length];

				memcpy(char_arr, data.c_str(), char_length ); //Can mem to game.

				//WideCharToMultiByte(CP_UTF8,0,data,)

				log << "Parse Data:(10)Address:" << int_addr << "&Data:" << char_arr;
				/*log << '\n';
				for (int i = 0; i < char_length; i++) {

					log << char_arr[i] << ' ';
				}*/

				DWORD ret;

				VirtualProtect((LPVOID)int_addr, sizeof(int), PAGE_EXECUTE_READWRITE, &ret);

				log << ",Alloc Size:" << ret;

				int read_addr = 0;
				ReadProcessMemory(process,(LPVOID)int_addr,&read_addr,4,NULL);

				log << ",Read Addr:" << read_addr;
				VirtualProtect((LPVOID)read_addr, char_length, PAGE_EXECUTE_READWRITE, &ret);
				SIZE_T written;

				if (WriteProcessMemory(process, (LPVOID)read_addr, char_arr, char_length, &written)) {
					log << ",Success!\n";
					char* test_char_arr = new char[char_length];
					ReadProcessMemory(process, (LPVOID)read_addr, test_char_arr, char_length, &written);

					log << "Debug Read:" << test_char_arr << '\n';
					
					//delete[] char_arr;
					//delete[] test_char_arr;
					//delete& address;
					//delete& data;
					//delete& line;

				}
				else {
					log << ",Faild!\n";
				}



			}
		}
		else {
			log << "Cannot Load GECV_STR.bin\n";
		}





	}

	return TRUE;
}

