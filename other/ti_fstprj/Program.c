// C Source File
// Created 10.07.2022; 16:57:33

// Delete or comment out the items you do not need.
#define COMMENT_STRING         "FirstProgram_string"
#define COMMENT_PROGRAM_NAME   "FirstProgram"
#define COMMENT_VERSION_STRING "v1"
#define COMMENT_VERSION_NUMBER 1,0,0,0 /* major, minor, revision, subrevision */
#define COMMENT_BW_ICON \
	{0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000}
#define COMMENT_GRAY_ICON \
	{0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000}, \
	{0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000, \
	 0b0000000000000000}

#define USE_TI89

#include <tigcclib.h>

// FORCE EXIT: 2nd + left + right + on (TI-89)
// Main Function
void _main(void)
{
	FILE *video = fopen("video.m", "r");
	if (video == NULL)
	{
		printf("Couldn't open file");
		return;
	}
	
	printf("Opened file");
	fclose(video);
	

	/*	
	printf("%d\n", LCD_LINE_BYTES);
	ngetchx();
		
  unsigned char *buffer = LCD_MEM;
  unsigned char black = 255;
	
	int arrSize = LCD_SIZE;
	
	int i = 0;
	for (; i < arrSize; i++)
	{
		buffer[i] = black;
	}
	
	printf("DONE! Iterations count: %d", i);
	ngetchx();
	*/
}
























