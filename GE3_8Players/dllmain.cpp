// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include <windows.h>
#include <Shlwapi.h>
#include <fstream>
#include <iostream>
#include <filesystem>
#include <tlhelp32.h>
#include <Psapi.h>
#include <string>
#pragma comment( lib, "Shlwapi.lib")

#pragma comment(linker, "/EXPORT:D3DReflect=AheadLib_D3DReflect")


extern "C"
{
    PVOID pfnAheadLib_D3DReflect;
}

std::wofstream llog;

static
HMODULE g_OldModule = NULL;

VOID WINAPI Free()
{
    if (g_OldModule)
    {
        FreeLibrary(g_OldModule);
    }
}

//Get all module related info, this will include the base DLL. 
//and the size of the module
MODULEINFO GetModuleInfo(wchar_t* szModule)
{
    MODULEINFO modinfo = { 0 };
    HMODULE hModule = GetModuleHandle(szModule);
    if (hModule == 0)
        return modinfo;
    GetModuleInformation(GetCurrentProcess(), hModule, &modinfo, sizeof(MODULEINFO));
    return modinfo;
}


void WriteToMemory(uintptr_t addressToWrite, char* valueToWrite, int byteNum)
{
    //used to change our file access type, stores the old
    //access type and restores it after memory is written
    unsigned long OldProtection;
    //give that address read and write permissions and store the old permissions at oldProtection
    VirtualProtect((LPVOID)(addressToWrite), byteNum, PAGE_EXECUTE_READWRITE, &OldProtection);

    //write the memory into the program and overwrite previous value
    memcpy((LPVOID)addressToWrite, valueToWrite, byteNum);

    //reset the permissions of the address back to oldProtection after writting memory
    VirtualProtect((LPVOID)(addressToWrite), byteNum, OldProtection, NULL);
}


DWORD FindPattern(wchar_t* module, char* pattern, char* mask)
{
    llog << "Start Find!" << "\n";

    //Get all module related information
    MODULEINFO mInfo = GetModuleInfo(module);

    //Assign our base and module size
    //Having the values right is ESSENTIAL, this makes sure
    //that we don't scan unwanted memory and leading our game to crash
    DWORD base = (DWORD)mInfo.lpBaseOfDll;
    DWORD size = (DWORD)mInfo.SizeOfImage;

    //Get length for our mask, this will allow us to loop through our array
    DWORD patternLength = (DWORD)strlen(mask);

    for (DWORD i = 0; i < size - patternLength; i++)
    {
        bool found = true;
        for (DWORD j = 0; j < patternLength; j++)
        {
            //if we have a ? in our mask then we have true by default, 
            //or if the bytes match then we keep searching until finding it or not


            llog << "i:" << i << ",j" << j << "\n";
            llog.flush();

            found &= mask[j] == '?' || pattern[j] == *(char*)(base + i + j);
        }

        //found = true, our entire pattern was found
        //return the memory addy so we can write to it
        if (found)
        {
            return base + i;
        }
    }

    return NULL;
}

//Define all Here, its easier.
char BytesToPatch[] = "\x08"; //What we are replacing with, for example \x90\ = NOP.
wchar_t ProcessName[] = L"ge3.exe"; //Processname
char BytePattern[] = "\xC7\x43\x04\x04\x00\x00\x00\xC7"; //Our Pattern
char ByteMask[] = "xx??xxxx"; //Our Mask
int Position = 0; 
int NoOfBytes = 2;

//Our Main Function
void InitiatePatch()
{
    DWORD Bytes = FindPattern(ProcessName, BytePattern, ByteMask);
    Bytes += Position;
    WriteToMemory(Bytes, BytesToPatch, NoOfBytes);

}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    
    TCHAR system_path[MAX_PATH]; 

    DisableThreadLibraryCalls(hModule);

    

    if (ul_reason_for_call == DLL_PROCESS_ATTACH) { 
    
        llog.open("GECV_8_PLAYERS.log", std::ios::trunc | std::ios::out);
        //if (!GetSystemDirectoryA(system_path, sizeof(system_path))) { //Always ASCII

        //    MessageBox(0, L"Cannot Get System d3dcompiler_47.dll Path", L"Error!", MB_OK);

        //    return false;


        //}
        //else {
        //    log << "Get Library:" << system_path << '\n';
        //}

        GetModuleFileName(NULL, system_path, MAX_PATH);
        PathRemoveFileSpec(system_path);

        lstrcat(system_path, TEXT("\\d3dcompiler_47.o.dll"));
        llog << "Get Library:" << system_path << '\n';
        
        llog.flush();



        HMODULE g_OldModule = LoadLibrary(system_path);
        
        if (g_OldModule == NULL) {
            llog << "Get Library Error!\n";
        }
        else {
            llog << "Module:" << g_OldModule << '\n';
        }

        //pfnAheadLib_D3DReflect = GetAddress("D3DReflect");
        pfnAheadLib_D3DReflect = GetProcAddress(g_OldModule, "D3DReflect");

        if (pfnAheadLib_D3DReflect == NULL)
        {
            llog << "Error Get Address:" << pfnAheadLib_D3DReflect << '\n';
            MessageBox(NULL, TEXT("Cannot get d3dcompiler_47.o.dll, Cannot Running!"), TEXT("GECV_8_PLAYERS"), MB_ICONSTOP);
            ExitProcess(-2);
        }
        else {
            llog << "Get Address:" << pfnAheadLib_D3DReflect << '\n';
        }

        HANDLE hProcess;
        hProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, FALSE, GetCurrentProcessId());
    
        if (hProcess)
        {
            llog << "Get Process:" << hProcess << '\n';
            llog.flush();
            InitiatePatch();
        }
        else {
            return false;
        }
    
        llog.flush();

    }


    return TRUE;
}
