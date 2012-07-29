typedef struct houghCircle_t 
{
	int x;
	int y;
	unsigned char intensity;
	int radius;
	double relativeIntensity;
} houghCircle;
 
// max 20 circles..
const int _maxCircles = 20;
houghCircle _circles[20];
int _circlesFound = 0;
int _circlesAdded = 0;
int _localPeakRadius = 4;
short _minCircleIntensity = 10;
short _maxMapIntensity = 0;
 
long _its = 0;
 
// Set point
//void SetHoughPoint(int x, int y, int width, int height, unsigned char *houghArray)		
void SetHoughPoint(void* par0, int* par1, unsigned char* par2)
{
	int x = par1[0];
	int y = par1[1];
	int width = par1[2];
	int height = par1[3];
	unsigned char* houghArray = par2;

    if ((x &gt;= 0) && (y &gt;= 0) && (x &lt; width) && (y &lt; height))
    {
		if(houghArray[y * width + x] &lt; 255)
		{
			houghArray[y* width + x]++;
		}
    }
}
 
// Set circle points
void SetHoughirclePoints(int cx, int cy, int x, int y, int width, int height, unsigned char *houghArray)
{
    if (x == 0)
    {
        SetHoughPoint(cx, cy + y, width, height, houghArray);
        SetHoughPoint(cx, cy - y, width, height, houghArray);
        SetHoughPoint(cx + y, cy, width, height, houghArray);
        SetHoughPoint(cx - y, cy, width, height, houghArray);
    }
    else if (x == y)
    {
        SetHoughPoint(cx + x, cy + y, width, height, houghArray);
        SetHoughPoint(cx - x, cy + y, width, height, houghArray);
        SetHoughPoint(cx + x, cy - y, width, height, houghArray);
        SetHoughPoint(cx - x, cy - y, width, height, houghArray);
    }
    else if (x &lt; y)
    {
        SetHoughPoint(cx + x, cy + y, width, height, houghArray);
        SetHoughPoint(cx - x, cy + y, width, height, houghArray);
        SetHoughPoint(cx + x, cy - y, width, height, houghArray);
        SetHoughPoint(cx - x, cy - y, width, height, houghArray);
        SetHoughPoint(cx + y, cy + x, width, height, houghArray);
        SetHoughPoint(cx - y, cy + x, width, height, houghArray);
        SetHoughPoint(cx + y, cy - x, width, height, houghArray);
        SetHoughPoint(cx - y, cy - x, width, height, houghArray);
    }
}
 
void DrawHoughCircle(int xcenter, int ycenter, int radiusToDetect, int width, int height, unsigned char *houghArray)
{
    int x = 0;
    int y = radiusToDetect;
    int p = (5 - radiusToDetect * 4) / 4;
 
    SetHoughirclePoints(xcenter, ycenter, x, y, width, height, houghArray);
 
    while (x &lt; y)
    {
        x++;
        if (p &lt; 0)
        {
            p += 2 * x + 1;
        }
        else
        {
            y--;
            p += 2 * (x - y) + 1;
        }
 
        SetHoughirclePoints(xcenter, ycenter, x, y, width, height, houghArray);
		_its++;
    }
 
 
}
 
int GetCircleX(unsigned char *generalArray, void **args, unsigned int argsCount, unsigned int *argSize)
{
	int circle = *(int*)args[0];
	if(circle &lt; _circlesAdded)
	{
		return _circles[circle].x;
	}
 
	return -1;
}
 
int GetCircleY(unsigned char *generalArray, void **args, unsigned int argsCount, unsigned int *argSize)
{
	int circle = *(int*)args[0];
	if(circle &lt; _circlesAdded)
	{
		return _circles[circle].y;
	}
 
	return -1;
}
 
int GetCircleRadius(unsigned char *generalArray, void **args, unsigned int argsCount, unsigned int *argSize)
{
	int circle = *(int*)args[0];
	if(circle &lt; _circlesAdded)
	{
		return _circles[circle].radius;
	}
 
	return -1;
}
 
int GetCircleIntensity(unsigned char *generalArray, void **args, unsigned int argsCount, unsigned int *argSize)
{
	int circle = *(int*)args[0];
	if(circle &lt; _circlesAdded)
	{
		return _circles[circle].intensity * 10;
	}
 
	return -1;
}
 
int GetCircleRelativeIntensity(unsigned char *generalArray, void **args, unsigned int argsCount, unsigned int *argSize)
{
	int circle = *(int*)args[0];
	if(circle &lt;= _circlesAdded)
	{
		return _circles[circle].relativeIntensity * 10;
	}
 
	return -1;
}
 
void CollectCircles(int width, int height, unsigned char *houghArray, float minIntensity, int radius) 
{
	_circlesFound = 0;
	_circlesAdded = 0;
	int circle = 0;
	int y = 0;
	int x = 0;
	int ty = 0;
	int tx = 0;
	int tyMax = 0;
	int txMax = 0;
 
    unsigned char intensity;
    short foundGreater;
 
    // clean circles collection
    for(x = 0; x &lt; _maxCircles; x++) 
	{
		_circles[x].x = 0;
		_circles[y].y = 0;
	}
 
 
 
    // for each Y coordinate
    for (y = 0; y &lt; height; y++)
    {
        // for each X coordinate
        for (x = 0; x &lt; width; x++)
        {
            // get current value
            intensity = houghArray[y * width + x];
 
            if (intensity &lt; _minCircleIntensity)
                continue;
 
            foundGreater = 0;
 
            // check neighboors
            for (ty = y - _localPeakRadius, tyMax = y + _localPeakRadius; ty &lt; tyMax; ty++)
            {
                // continue if the coordinate is out of map
                if (ty &lt; 0)
                    continue;
                // break if it is not local maximum or coordinate is out of map
                if ((foundGreater == 1) || (ty &gt;= height))
                    break;
 
                for (tx = x - _localPeakRadius, txMax = x + _localPeakRadius; tx &lt; txMax; tx++)
                {
                    // continue or break if the coordinate is out of map
                    if (tx &lt; 0)
                        continue;
                    if (tx &gt;= width)
                        break;
 
                    // compare the neighboor with current value
                    if (houghArray[ty * width + tx] &gt; intensity)
                    {
                        foundGreater = 1;
                        break;
                    }
                }
            }
 
            // was it local maximum ?
            if (foundGreater == 0 && ((double) intensity / _maxMapIntensity &gt;= minIntensity))
            {
				_circlesFound++;
				if(circle &lt; 20) // max 20 circles
				{
					_circles[circle].x = x;
					_circles[circle].y = y;
					_circles[circle].radius = radius;
					_circles[circle].intensity = intensity;
					_circles[circle].relativeIntensity = (double) intensity / _maxMapIntensity;
					circle++;
					_circlesAdded++;
				} else
				{
					return; // too many circles found..
				}
            }
        }
    }
 
 
 
	// .. this probably doesn't work; sorting like this doesn't do a deep copy of the objects
	// .. sorting of the circles are really not needed anyways.
	// .. commented out for now
	//houghCircle tmp;
	//for(x = 0; x &lt; circlesFound; x++)
	//for(y = 0; y &lt; circlesFound; y++)
	//{
	//	if(circles[x].intensity &lt; circles[y].intensity)
	//	{
	//		tmp = circles[x];
	//		circles[x] = circles[y];
	//		circles[y] = tmp;
	//	}
	//}
}
 
int ProcessImage(unsigned char *generalArray, void **args, unsigned int argsCount, unsigned int *argSize)
{
	unsigned char *imgArray = (unsigned char*)args[0];
	unsigned char *houghArray = (unsigned char*)args[1];
	unsigned int houghArraySize_c = argSize[1];
	unsigned int imgArraySize_c = argSize[0];
	int width = *(int*)args[2];
	int height = *(int*)args[3];
	int radius = *(int*)args[4];
	float intensity = *(float*)args[5];
 
	int x = 0;
	int y = 0;
	int b = 0;
	int offset = 0;
	_its = 0;
	int density = 0;
	// if more than 50% of the pixels are set; too many to detect any circles..
	// or if there are few pixels lit, we skip also
	// this is just for perf opt
	for(x = 0; x &lt; imgArraySize_c; x++) 
	{
        for (b = 0; b &lt; 8; b++)
        {
            if (((imgArray[x] >> b) & 1) != 0)
			{
				density++;
			}
		}
	}
 
	if(density &gt; ((imgArraySize_c * 8) / 2) || density &lt; 10) 
	{
		// too many or few pixels pixels lit; we don't even try to find any circles
		return density;
	}
 
	// clear out the houghmap incase it's not empty
	for(x = 0; x &lt; houghArraySize_c; x++) 
	{
		houghArray[x] = 0;
	}
 
	offset = 0;
    for (y = 0; y &lt; height; y++)
    {        
        for (x = 0; x &lt; width / 8; x++)
        {
			// each byte = 8 pixels. monochrome image
            for (b = 0; b &lt; 8; b++)
            {
                if (((imgArray[offset] >> b) & 1) != 0)
                {
					_its++;
                    DrawHoughCircle((x * 8) + b, y, radius, width, height, houghArray);                            
                }
 
            }
            offset++;        
        }
    }
 
    // find max value in Hough map
    _maxMapIntensity = 0;
    for (x = 0; x &lt; houghArraySize_c; x++)
    {
        if (houghArray[x] &gt; _maxMapIntensity)
        {
            _maxMapIntensity = houghArray[x];
        }
    }
 
	CollectCircles(width, height, houghArray, intensity, radius);
 
	return _circlesFound;
}