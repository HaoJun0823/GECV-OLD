// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include <Windows.h>
#include <fstream>
#include <filesystem>

namespace fs = std::filesystem;
static std::ofstream logg;

BYTE OLD_hook_bytes[5];
BYTE NEW_hook_bytes[5];
HANDLE process;
FARPROC window_hook;

BOOL
WINAPI
Hook_ShowWindow(
    _In_ HWND hWnd,
    _In_ int nCmdShow);


void Setup(bool b) {
    logg << "Hook Setup:" << b << "\n";
    DWORD memory_ret = 0;
    if (b) {
        WriteProcessMemory(process, window_hook, NEW_hook_bytes, 5, &memory_ret);
        logg << "Write True Hook Result:" << memory_ret << "\n";
    }
    else {
        WriteProcessMemory(process, window_hook, OLD_hook_bytes, 5, &memory_ret);
        logg << "Write False Hook Result:" << memory_ret << "\n";
    }
    
    
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{

    if(ul_reason_for_call == DLL_PROCESS_ATTACH) {

        logg.open(".\\CE_DATA\\controller_fix.log", std::ios::trunc | std::ios::out);


        window_hook = GetProcAddress(GetModuleHandleA("user32"), "ShowWindow");

        process = OpenProcess(PROCESS_ALL_ACCESS, FALSE, GetCurrentProcessId());

        logg << "Success Get Hook Address!\n";
        DWORD memory_ret = 0;
        ReadProcessMemory(process, window_hook, OLD_hook_bytes, 5, &memory_ret);
        logg << "Read Hook Result:" << memory_ret << "\n";
        NEW_hook_bytes[0] = '\xE9';

        DWORD offset = (DWORD)Hook_ShowWindow - (DWORD)window_hook - 5;

        memcpy_s(&NEW_hook_bytes[1], 4, &offset, 4);

        VirtualProtect(window_hook, 5, PAGE_EXECUTE_READWRITE, &memory_ret);
        logg << "Protect Hook Result:" << memory_ret << "\n";
        logg << "Install Hook.\n";
        Setup(true);


        DisableThreadLibraryCalls(hModule);
        


        
        logg.flush();
    }

    return true;

}

BOOL
WINAPI
Hook_ShowWindow(
    _In_ HWND hWnd,
    _In_ int nCmdShow) {
    Setup(false);
    logg << "Current hwnd:" << hWnd << "\n";
    SetForegroundWindow(NULL);
    logg << "SetForegroundWindow To Null.\n";
    for (int i = 0; i < 1; i++) {
        SetFocus(NULL);
        Sleep(100);
        SetFocus(hWnd);
        logg << "Focus Count:" << i << "\n";
    }
    SetForegroundWindow(hWnd);
    logg << "SetForegroundWindow To Game.\nDone!\n";
    logg.flush();
    BOOL result =  ShowWindow(hWnd, nCmdShow);
    Setup(true);
    logg << "Original Result:" << result << "\n";
    return result;
}