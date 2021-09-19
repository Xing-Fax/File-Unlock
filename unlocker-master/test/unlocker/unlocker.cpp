#pragma once

#include <stdio.h>

#include "..\..\unlocker.hpp"

#include <cstdlib>

// main
int _tmain(int argc, _TCHAR* argv[])
{
	unlocker::File* file = unlocker::Path::Exists((argv[1]));
	if (file) {
		file->Unlock();
		delete file;
	}
	return 0;
}
