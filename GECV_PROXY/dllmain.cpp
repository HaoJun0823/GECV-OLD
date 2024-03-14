// dllmain.cpp : 定义 DLL 应用程序的入口点。
//#include "pch.h"
#include <windows.h>
#include <fstream>
#include <iostream>
#include <vector>
#include <filesystem>

namespace fs = std::filesystem;

#pragma comment(linker, "/EXPORT:DirectInput8Create=_SHADOW_DirectInput8Create") //_ = This Dll



PVOID SHADOW_POINT_DirectInput8Create;
BOOL WINAPI Inject(HANDLE p);

BOOL __stdcall DllMain(HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    //switch (ul_reason_for_call)
    //{
    //case DLL_PROCESS_ATTACH:
    //case DLL_THREAD_ATTACH:
    //case DLL_THREAD_DETACH:
    //case DLL_PROCESS_DETACH:
    //    break;
    //}
    //return TRUE;

    


    if (ul_reason_for_call == DLL_PROCESS_ATTACH) {

        
        std::ofstream log;
        log.open("GECV_PROXY.log",std::ios::trunc | std::ios::out);
        

        if (!log.is_open()) {

            MessageBox(0, L"Cannot Create Log System", L"Error!", MB_OK);
            return false;
        }
        else {

            log << "Hello Game!\n";

        }
        

        char system_path[MAX_PATH];


        if (!GetSystemDirectoryA(system_path, sizeof(system_path))) { //Always ASCII

            MessageBox(0, L"Cannot Get System dinput8.dll Path", L"Error!", MB_OK);

            return false;


        }
        else {
            log << "Get Library:" << system_path << '\n';
        }

        strcat_s(system_path, "\\dinput8.dll");

        HMODULE library = LoadLibraryA(system_path);
        


        if (library == NULL) {

            MessageBox(0, L"Cannot Load System dinput8.dll", L"Error!", MB_OK);
            return false;
        }
        else {
            log << "Open Library:" << &library << '\n';
        }
        
        FARPROC proc = GetProcAddress(library,"DirectInput8Create");

        if (proc == NULL) {
            MessageBox(0, L"Cannot Get System dinput8.dll Function Address:DirectInput8Create", L"Error!", MB_OK);
        }
        else {
            SHADOW_POINT_DirectInput8Create = proc;
            log << "Get DirectInput8Create:" << &SHADOW_POINT_DirectInput8Create << '\n';
        }


        HANDLE process = OpenProcess(PROCESS_ALL_ACCESS, FALSE, GetCurrentProcessId());
        
        log << "Current Process:" << GetCurrentProcessId() << "\n";

        log.close();
        Inject(process);
        
    }


    



    return TRUE;

}


BOOL WINAPI Inject(HANDLE p) {

    
    std::ofstream log;
    log.open("GECV_PROXY.log", std::ios::app | std::ios::out);

    HMODULE hKernel32 = GetModuleHandle(L"kernel32.dll");
    if (hKernel32 == nullptr) {
        log << "Cannot Get kernel32.dll!" << '\n';
        return false;
    }
    FARPROC loadLibraryAddr = (FARPROC)GetProcAddress(hKernel32, "LoadLibraryW");
    if (loadLibraryAddr == nullptr) {
        log << "Cannot Get kernel32.dll! LoadLibraryW" << '\n';
        return false;
    }



    std::vector<fs::path> files;

    if (!fs::exists(".\\CE_DATA")) {
        fs::create_directory("CE_DATA");
    }

    
    for (const auto& item : fs::recursive_directory_iterator(".\\CE_DATA\\")) {


        if (item.path().extension() == ".dll") {

            log << "Get Plugins:" << item.path().generic_string() << '\n';
            files.push_back(item.path());

        }

    }


    for (const auto& item : files) {


        const wchar_t* dllpath = item.c_str();

        int char_length = (wcsnlen_s(dllpath,4096) + 1) * sizeof(wchar_t);

        SIZE_T written_count  = 0;

        log << "Load Plugins:" << item.generic_string() << '\n';

        void* remote_alloc = VirtualAllocEx(p, nullptr, char_length, MEM_COMMIT, PAGE_READWRITE);

        if (remote_alloc == nullptr) {


            log << "Cannot Alloc Memory:" << item.generic_string() << '\n';

        }
        else if (!WriteProcessMemory(p,remote_alloc,(void*)dllpath,char_length,&written_count)) {
            log << "Cannot Write Dll To Memory :Length " << item.generic_string() << '\n';
            VirtualFreeEx(p, remote_alloc, 0, MEM_RELEASE);
        }
        else {
            log << "Write Dll To Memory:" << written_count << " At " << remote_alloc << '\n';
            DWORD thread_id;
            HANDLE thread = CreateRemoteThread(p,nullptr,0,(LPTHREAD_START_ROUTINE)loadLibraryAddr,remote_alloc,0,&thread_id);

            if (thread == nullptr) {
                log << "Cannot Create Thread:" << item.generic_string() << '\n';
                VirtualFreeEx(p, remote_alloc, 0, MEM_RELEASE);
            }
            else {
                log << "Create Thread:" << item.generic_string() << '\n';

                //WaitForSingleObject(thread, INFINITE);
            }


        }



    }
    log << "Done!\n";

    return true;

}


EXTERN_C __declspec(naked) void __cdecl SHADOW_DirectInput8Create(void)
{
    __asm jmp SHADOW_POINT_DirectInput8Create;
}