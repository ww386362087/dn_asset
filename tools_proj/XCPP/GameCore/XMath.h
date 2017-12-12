#ifndef __XMATH_H__
#define __XMATH_H__

#define XPI	3.1415926f
#define XRound 360.0f
#define XDeg2Rad(x) ((x) * 0.0174533f)
#define XRad2Deg(x) ((x) * 57.295780f)
#define XRoundToCircle(x) (float(((int)(x) % (int)XRound) + (float)((x) - (int)(x))))
#define XIsNaN(x) (assert(!_isnan(x)))
#define XClamp(x, minValue, maxValue) (std::max(std::min((x), (maxValue)), (minValue)))
#define XIsInteger(x) ((std::abs(x - (int)x) < 0.0001f) || (std::abs(x - (int)x) > (1 - 0.0001f)))

#include <math.h>

#endif	//__XMATH_H__