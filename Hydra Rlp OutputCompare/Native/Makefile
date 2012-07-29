####################################################
###### Copyright(c) GHI Electronics, LLC ######
####################################################

OUTFILE=SignalGen
LINKERSCRIPT = Linker.lds


INCL=./include

CC		=arm-none-eabi-gcc
LD		=arm-none-eabi-gcc

CCFLAGS=  -g -mlittle-endian -mcpu=arm7tdmi  -Wall -I. -I$(INCL)
CCFLAGS+= -mapcs-frame -fno-builtin -g0

LDFLAGS =-nostartfiles -Wl,--Map -Wl,./$(OUTFILE).map
LDFLAGS+=-lc -lgcc -Wl,--omagic
LDFLAGS+=-T $(LINKERSCRIPT)

OBJS+= SignalGen.o \

rebuild: clean all del_o

all: $(OBJS)
	$(LD) $(LDFLAGS) -o ../Managed/$(OUTFILE).elf $(OBJS)
	

SignalGen.o: SignalGen.c 
	$(CC) -c $(CCFLAGS) SignalGen.c -o SignalGen.o

clean:
	-rm *.o ../Managed/*.elf ./*.map

del_o:
	-rm *.o

del_map:
	-rm ./*.map