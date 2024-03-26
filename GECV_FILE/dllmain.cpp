// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include <Windows.h>
#include <fstream>
#include <iostream>
#include <filesystem>
#include <string>
#include <Shlwapi.h>

#include "hooker.h"

#pragma comment(lib, "Shlwapi.lib")

namespace fs = std::filesystem;


static HANDLE process;

static Hooker Hooker_CreateFileW;
static Hooker Hooker_CreateFileA;

static Hooker Hooker_PathFileExistsA;
static Hooker Hooker_PathFileExistsW;

static Hooker Hooker_OutputDebugStringA;


HANDLE
WINAPI
Hook_CreateFileA(
    _In_ LPCSTR lpFileName,
    _In_ DWORD dwDesiredAccess,
    _In_ DWORD dwShareMode,
    _In_opt_ LPSECURITY_ATTRIBUTES lpSecurityAttributes,
    _In_ DWORD dwCreationDisposition,
    _In_ DWORD dwFlagsAndAttributes,
    _In_opt_ HANDLE hTemplateFile
);

HANDLE
WINAPI
Hook_CreateFileW(
    _In_ LPCWSTR lpFileName,
    _In_ DWORD dwDesiredAccess,
    _In_ DWORD dwShareMode,
    _In_opt_ LPSECURITY_ATTRIBUTES lpSecurityAttributes,
    _In_ DWORD dwCreationDisposition,
    _In_ DWORD dwFlagsAndAttributes,
    _In_opt_ HANDLE hTemplateFile
);

VOID
WINAPI
Hook_OutputDebugStringA(
    _In_opt_ LPCSTR lpOutputString
);

BOOL Hook_PathFileExistsA(_In_ LPCSTR pszPath);
BOOL Hook_PathFileExistsW(_In_ LPCWSTR pszPath);

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    if (ul_reason_for_call == DLL_PROCESS_ATTACH) {


        fs:remove(".\\CE_DATA\\GAME_DEBUG.log");

        std::ofstream log;
        log.open(".\\CE_DATA\\GECV_FILE.log", std::ios::trunc | std::ios::out);
        log.close();
        DisableThreadLibraryCalls(hModule);

        FARPROC File_Hook_A = GetProcAddress(GetModuleHandleA("kernel32"), "CreateFileA");
        FARPROC File_Hook_W = GetProcAddress(GetModuleHandleA("kernel32"), "CreateFileW");

        FARPROC Path_Hook_A = GetProcAddress(GetModuleHandleA("shlwapi"), "PathFileExistsA");
        FARPROC Path_Hook_W = GetProcAddress(GetModuleHandleA("shlwapi"), "PathFileExistsW");

        FARPROC Output_Debug_A = GetProcAddress(GetModuleHandleA("kernel32"), "OutputDebugStringA");

        

        process = OpenProcess(PROCESS_ALL_ACCESS, FALSE, GetCurrentProcessId());


        Hooker_CreateFileA.Create(process, File_Hook_A, (DWORD)Hook_CreateFileA);
        Hooker_CreateFileW.Create(process, File_Hook_W, (DWORD)Hook_CreateFileW);

        Hooker_PathFileExistsA.Create(process, Path_Hook_A, (DWORD)Hook_PathFileExistsA);
        Hooker_PathFileExistsW.Create(process, Path_Hook_W, (DWORD)Hook_PathFileExistsW);

        Hooker_OutputDebugStringA.Create(process, Output_Debug_A,(DWORD)Hook_OutputDebugStringA);

        log << "Created Hook.\n";

        Hooker_CreateFileA.Install();
        Hooker_CreateFileW.Install();
        Hooker_PathFileExistsA.Install();
        Hooker_PathFileExistsW.Install();
        Hooker_OutputDebugStringA.Install();
        
        log << "Install Hook.\n";
        
        //Hooker_CreateFileA.UnInstall();
        //Hooker_CreateFileW.UnInstall();

        //log << "UnInstall Hook.\n";

        


    }

    return true;
}



HANDLE
WINAPI
Hook_CreateFileA(
    _In_ LPCSTR lpFileName,
    _In_ DWORD dwDesiredAccess,
    _In_ DWORD dwShareMode,
    _In_opt_ LPSECURITY_ATTRIBUTES lpSecurityAttributes,
    _In_ DWORD dwCreationDisposition,
    _In_ DWORD dwFlagsAndAttributes,
    _In_opt_ HANDLE hTemplateFile
) {
    
    std::ofstream log;
    log.open(".\\CE_DATA\\GECV_FILE.log", std::ios::app | std::ios::out);

    log << "CreateFileA:" << lpFileName << "\n";

    Hooker_CreateFileA.UnInstall();
    HANDLE result =  CreateFileA(lpFileName,dwDesiredAccess,dwShareMode,lpSecurityAttributes,dwCreationDisposition,dwFlagsAndAttributes,hTemplateFile);
    Hooker_CreateFileA.Install();

    log.flush();
    log.close();

    return result;
}

HANDLE
WINAPI
Hook_CreateFileW(
    _In_ LPCWSTR lpFileName,
    _In_ DWORD dwDesiredAccess,
    _In_ DWORD dwShareMode,
    _In_opt_ LPSECURITY_ATTRIBUTES lpSecurityAttributes,
    _In_ DWORD dwCreationDisposition,
    _In_ DWORD dwFlagsAndAttributes,
    _In_opt_ HANDLE hTemplateFile
) {
    

    std::ofstream log;
    log.open(".\\CE_DATA\\GECV_FILE.log", std::ios::app | std::ios::out);

    log << "CreateFileW:" << lpFileName << "\n";

    Hooker_CreateFileW.UnInstall();
    
    
    HANDLE result = CreateFileW(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);\
    Hooker_CreateFileW.Install();

    log.flush();
    log.close();

    return result;
}

BOOL Hook_PathFileExistsA(_In_ LPCSTR pszPath) {

    std::ofstream log;
    log.open(".\\CE_DATA\\GECV_FILE.log", std::ios::app | std::ios::out);

    log << "PathFileExistsA:" << pszPath << "\n";
    log.flush();
    log.close();

    Hooker_PathFileExistsA.UnInstall();

    BOOL result = PathFileExistsA(pszPath);
    Hooker_PathFileExistsA.Install();
    return result;
}
BOOL Hook_PathFileExistsW(_In_ LPCWSTR pszPath) {
    std::ofstream log;
    log.open(".\\CE_DATA\\GECV_FILE.log", std::ios::app | std::ios::out);

    log << "PathFileExistsW:" << pszPath << "\n";
    log.flush();
    log.close();

    Hooker_PathFileExistsW.UnInstall();
    BOOL result = PathFileExistsW(pszPath);
    Hooker_PathFileExistsW.Install();
    return result;

}

VOID
WINAPI
Hook_OutputDebugStringA(
    _In_opt_ LPCSTR lpOutputString
){

    std::ofstream log;
    log.open(".\\CE_DATA\\GAME_DEBUG.log", std::ios::app | std::ios::out);

    log << "OutputDebugStringA:" << lpOutputString;
    log.flush();
    log.close();


    Hooker_OutputDebugStringA.UnInstall();

    OutputDebugStringA(lpOutputString);

    Hooker_OutputDebugStringA.Install();

    return ;
    


}