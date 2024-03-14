// dllmain.cpp : 定义 DLL 应用程序的入口点。
//#include "pch.h"
#include <windows.h>
#include <fstream>
#include <iostream>


BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    std::ofstream log;
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        log.open(".\\CE_DATA\\GECV_HELLO.log", std::ios::trunc | std::ios::out);
        log << "Hello Game For Process!\n";
        break;
    case DLL_THREAD_ATTACH:
        log.open(".\\CE_DATA\\GECV_HELLO.log", std::ios::app | std::ios::out);
        log << "Hello Game For Thread!\n";
        break;
    case DLL_THREAD_DETACH:
        log.open(".\\CE_DATA\\GECV_HELLO.log", std::ios::app | std::ios::out);
        log << "Bye Game For Thread!\n";
        break;
    case DLL_PROCESS_DETACH:
        log.open(".\\CE_DATA\\GECV_HELLO.log", std::ios::app | std::ios::out);
        log << "Bye Game For Process!\n";
        break;
    }

    log.close();



    return TRUE;
}

