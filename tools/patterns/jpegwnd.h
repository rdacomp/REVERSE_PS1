
typedef struct PatternEntry
{
    int     PatternIndex;
    int     PosX;           // ������������ �������� ������ ���� ������������� ����.
    int     PosY;
    int     SavedPosX;      // ������ ��������� ���� ����������� ���� ��� ����������
    int     SavedPosY;
    int     PlaneX;         // ������������ �������� ������ ���� �������� ��������.
    int     PlaneY;
    int     Width;
    int     Height;
    HWND    Hwnd;
    bool    Flipped;
    float   BlendLevel;     // UpdateLayeredWindow
} PatternEntry;

void JpegInit(HWND Parent);

void JpegLoadImage(char *filename, bool Silent);

bool JpegGetSelectRegion(LPRECT Region);

void JpegSetSelectRegion(LPRECT Region);

void JpegResize(int Width, int Height);

int JpegWindowWidth(void);

void AddPatternEntry(int PatternIndex);

void UpdatePatternEntry(int EntryIndex, PatternEntry * Entry);

void JpegRemoveSelection(void);

void JpegRedraw(void);

PatternEntry * GetPatternEntry(int EntryIndex);

int GetPatternEntryNum(void);

void JpegDestroy(void);

char * JpegGetImageName(void);

void JpegGetScroll(LPPOINT Offset);

void JpegSetScroll(LPPOINT Offset);
