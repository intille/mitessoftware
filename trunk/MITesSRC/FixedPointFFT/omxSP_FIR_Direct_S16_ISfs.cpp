/**
 * 
 * File Name:  omxSP_FIR_Direct_S16_ISfs.c
 * OpenMAX DL: v1.0.2
 * Revision:   8916
 * Date:       Wednesday, October 31, 2007
 * 
 * (c) Copyright 2007 ARM Limited. All Rights Reserved.
 * 
 * 
 * Description:
 *
 */

#include "stdafx.h"
#include "omxtypes.h"
#include "armOMX.h"
#include "omxSP.h"

extern 


/**
 * Function: omxSP_FIR_Direct_S16_ISfs
 *
 * Description:
 * Block FIR filtering for 16-bit data type.
 *
 * Remarks:
 * This function applies the FIR filter defined by the coefficient vector
 * pTapsQ15 to a vector of input data.
 *
 * Parameters:
 * [in]  pSrcDst        pointer to the vector of input samples to which
 *              the filter is applied.
 * [in]  sampLen        the number of samples contained in both the
 *              input and output vectors.
 * [in]  pTapsQ15       pointer to the vector that contains the filter
 *              coefficients, represented in Q0.15 format.
 *              Given that -32768<=pTapsQ15(k)<32768,
 *              0<=k<tapsLen, the range on the actual filter
 *              coefficients is: -1<=bk<1, and therefore
 *              coefficient normalization  may be required during
 *              the filter design process.
 * [in]  tapsLen        the number of taps, e.g. the filter order + 1
 * [in]  pDelayLine       pointer to the 2*tapsLen-element filter memory
 *              buffer(state). The user is responsible for
 *              allocation, initialization, and de-allocation.
 *              The filter memory elements are initialized to
 *              zero in most    applications.
 * [in]  pDelayLineIndex  pointer to the filter memory index that is
 *              maintained internally by the primitive. User
 *              should initialize the value of this index to
 *              zero.
 * [in]  scalefactor    saturation fixed scalefactor.
 * [out] pSrcDst        pointer to the filtered output samples.
 *
 * Return Value:
 * Standard omxError result. See enumeration for possible result codes.
 *
 */

OMXResult omxSP_FIR_Direct_S16_ISfs(
     OMX_S16 * pSrcDst,
     OMX_INT sampLen,
     const OMX_S16 * pTapsQ15,
     OMX_INT tapsLen,
     OMX_S16 * pDelayLine,
     OMX_INT * pDelayLineIndex,
     OMX_INT scaleFactor
 )
 {
	 return omxSP_FIR_Direct_S16_Sfs (pSrcDst, pSrcDst, sampLen, pTapsQ15, 
										tapsLen, pDelayLine, 
										pDelayLineIndex, scaleFactor);
 }


/*****************************************************************************
 *                              END OF FILE
 *****************************************************************************/

