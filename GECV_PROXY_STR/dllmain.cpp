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

		DWORD base_address = (DWORD)GetModuleHandle(NULL);

		log << "Base Address:" << std::hex << base_address << "\n";

		if (str_bin.is_open()) {
			log << "Load GECV_STR.bin\n";
			while (getline(str_bin, line)) {

				log << "Read Text:\"" << line << '\"' << '\n';

				string address = line.substr(0, 8);
				string data = line.substr(9);
				log << "Split Data:(16)Address:" << address << "&Data:" << data << '\n';

				
				string br = "<br>";
				string replace_br = "\n";

				size_t pos = 0;

				while ((pos = data.find(br, pos)) != string::npos) {

					data.replace(pos, br.length(), replace_br);

				}


				uint32_t int_addr = stoi(address, 0, 16);

				SIZE_T char_length = data.length() + 1;

				char* char_arr = new char[char_length];

				memcpy(char_arr, data.c_str(), char_length); //Can mem to game.

				//WideCharToMultiByte(CP_UTF8,0,data,)

				log << "Parse Data:(16)Address:" << std::hex << int_addr << ",Data:" << char_arr;

				int_addr += base_address;
				log << "+Base Address:" << int_addr;

				/*log << '\n';
				for (int i = 0; i < char_length; i++) {

					log << char_arr[i] << ' ';
				}*/

				DWORD ret = 0;

				if (VirtualProtectEx(process, (LPVOID)int_addr, sizeof(uint32_t), PAGE_EXECUTE_READWRITE, &ret)) {

					log << ",Protect Success!";

				}
				else {
					log << ",Protect Faild!";
				}


				log << ",Alloc Cursor Size:" << ret;

				uint32_t read_addr = 0;
				ReadProcessMemory(process, (LPVOID)int_addr, &read_addr, sizeof(uint32_t), NULL);

				log << ",Read Address:" << std::hex << read_addr;


				void* alloc_addr = VirtualAllocEx(process, NULL, char_length, MEM_COMMIT, PAGE_READWRITE);
				log << ",Alloc Address:" << std::hex << &alloc_addr;
				SIZE_T written;

				if (WriteProcessMemory(process, alloc_addr, (void*)char_arr, char_length, &written)) {
					log << ",Success!";
					//VirtualProtect((LPVOID)int_addr, char_length, PAGE_EXECUTE_READWRITE, &ret);
					log << ",Written Size:" << std::dec <<written;
					if (WriteProcessMemory(process, (LPVOID)int_addr, &alloc_addr, sizeof(uint32_t), &written)) {
						log << ",Success!" << "Written:" << written << '\n';
						
						

						uint32_t debug_read_addr = 0;

						ReadProcessMemory(process, (LPVOID)int_addr, &debug_read_addr, sizeof(uint32_t), &written);


						log << "Debug:Written Address Read:" << std::hex << debug_read_addr << ",Read Size:" << std::dec << written << '\n';

						char* test_char_arr = new char[char_length];

						ReadProcessMemory(process, alloc_addr, test_char_arr, char_length, &written);

						log << "Debug:Alloc Data Read:" << test_char_arr << ",Read Size:" <<std::dec << written << '\n';

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
		}
		else {
			log << "Cannot Load GECV_STR.bin\n";
		}


		log.flush();
		log.close();

	}



	return TRUE;
}

