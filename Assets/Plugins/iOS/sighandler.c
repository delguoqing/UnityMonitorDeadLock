#include <signal.h>
#include <execinfo.h>
#include <signal.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>
#include <unistd.h>

void (*c)(int);

void sig_callback(int n){
	c(n);
}

void RegSigHandler(int s, void (*handler)(int)){
	c = handler;			
	signal(s, sig_callback);
}

int ReadStackTrace(char* buf, int bufLen)
{
    void *callstack[128];
    int frames = backtrace(callstack, 128);
    char **strs = backtrace_symbols(callstack, frames);
    int i = 0;
    int j = 0;
    for (i = 0; i < frames; i++) {
        int lineLen = strlen(strs[i]);
        if (j + lineLen < bufLen) {
            memcpy(buf + j, strs[i], lineLen);
            j += lineLen;
        } else {
            break;
        }

        if (j + 1 < bufLen) {
            buf[j ++] = '\n';
        } else {
            break;
        }
    }
    free(strs);
    return j;
} 

void Signal(int s) {
    kill(getpid(), s);
}
