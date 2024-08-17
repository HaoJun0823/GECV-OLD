// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include <Windows.h>
#include <fstream>
#include <filesystem>
static DWORD game_baseaddress;
static DWORD dll_baseaddress;
static HMODULE dllModule;
static HANDLE process;
static std::ofstream logg;

static DWORD GE1_NOP_A;
static DWORD GE1_NOP_B;

static DWORD GE2_NOP_A;
static DWORD GE2_NOP_B;

namespace fs = std::filesystem;
BYTE hook_bytes[6] = { 0x90,0x90,0x90,0x90,0x90,0x90 };
void FixGE1();
void FixGE2();


BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{

    if (ul_reason_for_call == DLL_PROCESS_ATTACH) {


        logg.open(".\\CE_DATA\\game_menu_mod.log", std::ios::trunc | std::ios::out);


        process = OpenProcess(PROCESS_ALL_ACCESS, FALSE, GetCurrentProcessId());

        logg << "Process:" << std::hex << GetCurrentProcessId() << "\n";

        game_baseaddress = (DWORD)GetModuleHandle(NULL);
        logg << "Game Base Address:" << std::hex << game_baseaddress << "\n";
        dll_baseaddress = (DWORD)hModule;
        logg << "Dll Base Address:" << std::hex << game_baseaddress << "\n";
        dllModule = hModule;

        if (fs::exists(".\\ger.exe")) {
            logg << "This is God Eater 1\n";
            FixGE1();

        }
        else if (fs::exists(".\\ge2rb.exe")) {
            logg << "This is God Eater 2\n";
            FixGE2();
        }
        else {

            logg << "Get Game Error! No Fixed Game!\n";

            //MessageBox(0, L"Unofficial patch: Your game is not recognized.", L"Where is the game?", MB_OK);



        }

        logg.flush();

    }

    return TRUE;
}

void FixGE2() {

    GE1_NOP_A = game_baseaddress + 0X1B607F9;
    logg << "GE1 MENU_A Address:" << std::hex << GE1_NOP_A << "\n";
    GE1_NOP_B = game_baseaddress + 0X1BB3CA2;
    logg << "GE1 MENU_B Address:" << std::hex << GE1_NOP_B << "\n";
    DWORD ret;
    VirtualProtect(reinterpret_cast<LPVOID>(GE1_NOP_A), 6, PAGE_EXECUTE_READWRITE, &ret);
    WriteProcessMemory(process, reinterpret_cast<LPVOID>(GE1_NOP_A), hook_bytes, 6, &ret);
    VirtualProtect(reinterpret_cast<LPVOID>(GE1_NOP_B), 6, PAGE_EXECUTE_READWRITE, &ret);
    WriteProcessMemory(process, reinterpret_cast<LPVOID>(GE1_NOP_B), hook_bytes, 6, &ret);


}

void FixGE1() {

    GE2_NOP_A = game_baseaddress + 0X015B6CBB;
    logg << "GE2 MENU_A Address:" << std::hex << GE2_NOP_A << "\n";
    GE2_NOP_B = game_baseaddress + 0X0160CD39;
    logg << "GE2 MENU_B Address:" << std::hex << GE2_NOP_B << "\n";
    DWORD ret;
    VirtualProtect(reinterpret_cast<LPVOID>(GE2_NOP_A), 6, PAGE_EXECUTE_READWRITE, &ret);
    WriteProcessMemory(process, reinterpret_cast<LPVOID>(GE2_NOP_A), hook_bytes, 6, &ret);
    VirtualProtect(reinterpret_cast<LPVOID>(GE2_NOP_B), 6, PAGE_EXECUTE_READWRITE, &ret);
    WriteProcessMemory(process, reinterpret_cast<LPVOID>(GE2_NOP_B), hook_bytes, 6, &ret);

}