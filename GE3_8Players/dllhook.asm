
;
; created by AheadLib
; github:https://github.com/strivexjun/AheadLib-x86-x64
;
; �� .asm �ļ���ӵ�����һ��
; �Ҽ������ļ�-����-����-
; ������:�Զ������ɹ���
; ���������ų�:��

; Ȼ����������������
; ������: ml64 /Fo $(IntDir)%(fileName).obj /c /Cp %(fileName).asm
; ���: $(IntDir)%(fileName).obj;%(Outputs)
; ���Ӷ���: ��
;


.DATA
EXTERN pfnAheadLib_D3DReflect:dq;


.CODE
AheadLib_D3DReflect PROC
	jmp pfnAheadLib_D3DReflect
AheadLib_D3DReflect ENDP


END
