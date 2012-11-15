/*******************************************************************************

INTEL CORPORATION PROPRIETARY INFORMATION
This software is supplied under the terms of a license agreement or nondisclosure
agreement with Intel Corporation and may not be copied or disclosed except in
accordance with the terms of that agreement
Copyright(c) 2012 Intel Corporation. All Rights Reserved.

@Author {Blake C. Lucas (img.science@gmail.com)}
*******************************************************************************/

#define CONFIDENCE_THRESHOLD 100.0f
#define DEPTH_RANGE_THRESH 3
#define IR_RANGE_THRESH 10
#define MIN_CONFIDENCE 100

#define PIX_SORT(a,b) { if ((a)>(b)) PIX_SWAP((a),(b)); }
#define PIX_SWAP(a,b) { float temp=(a);(a)=(b);(b)=temp; }
kernel void CopyDepth(const global float4* depthData,global float4* depthBuffer){
	int index = get_global_id(0);
	depthBuffer[index]=depthData[index];
}
/*----------------------------------------------------------------------------
   Function :   opt_med25()
   In       :   pointer to an array of 25 pixelvalues
   Out      :   a float
   Job      :   optimized search of the median of 25 pixelvalues
   Notice   :   in theory, cannot go faster without assumptions on the
                signal.
  				Code taken from Graphic Gems.
 ---------------------------------------------------------------------------*/

float opt_med25(float * p)
{


    PIX_SORT(p[0], p[1]) ;   PIX_SORT(p[3], p[4]) ;   PIX_SORT(p[2], p[4]) ;
    PIX_SORT(p[2], p[3]) ;   PIX_SORT(p[6], p[7]) ;   PIX_SORT(p[5], p[7]) ;
    PIX_SORT(p[5], p[6]) ;   PIX_SORT(p[9], p[10]) ;  PIX_SORT(p[8], p[10]) ;
    PIX_SORT(p[8], p[9]) ;   PIX_SORT(p[12], p[13]) ; PIX_SORT(p[11], p[13]) ;
    PIX_SORT(p[11], p[12]) ; PIX_SORT(p[15], p[16]) ; PIX_SORT(p[14], p[16]) ;
    PIX_SORT(p[14], p[15]) ; PIX_SORT(p[18], p[19]) ; PIX_SORT(p[17], p[19]) ;
    PIX_SORT(p[17], p[18]) ; PIX_SORT(p[21], p[22]) ; PIX_SORT(p[20], p[22]) ;
    PIX_SORT(p[20], p[21]) ; PIX_SORT(p[23], p[24]) ; PIX_SORT(p[2], p[5]) ;
    PIX_SORT(p[3], p[6]) ;   PIX_SORT(p[0], p[6]) ;   PIX_SORT(p[0], p[3]) ;
    PIX_SORT(p[4], p[7]) ;   PIX_SORT(p[1], p[7]) ;   PIX_SORT(p[1], p[4]) ;
    PIX_SORT(p[11], p[14]) ; PIX_SORT(p[8], p[14]) ;  PIX_SORT(p[8], p[11]) ;
    PIX_SORT(p[12], p[15]) ; PIX_SORT(p[9], p[15]) ;  PIX_SORT(p[9], p[12]) ;
    PIX_SORT(p[13], p[16]) ; PIX_SORT(p[10], p[16]) ; PIX_SORT(p[10], p[13]) ;
    PIX_SORT(p[20], p[23]) ; PIX_SORT(p[17], p[23]) ; PIX_SORT(p[17], p[20]) ;
    PIX_SORT(p[21], p[24]) ; PIX_SORT(p[18], p[24]) ; PIX_SORT(p[18], p[21]) ;
    PIX_SORT(p[19], p[22]) ; PIX_SORT(p[8], p[17]) ;  PIX_SORT(p[9], p[18]) ;
    PIX_SORT(p[0], p[18]) ;  PIX_SORT(p[0], p[9]) ;   PIX_SORT(p[10], p[19]) ;
    PIX_SORT(p[1], p[19]) ;  PIX_SORT(p[1], p[10]) ;  PIX_SORT(p[11], p[20]) ;
    PIX_SORT(p[2], p[20]) ;  PIX_SORT(p[2], p[11]) ;  PIX_SORT(p[12], p[21]) ;
    PIX_SORT(p[3], p[21]) ;  PIX_SORT(p[3], p[12]) ;  PIX_SORT(p[13], p[22]) ;
    PIX_SORT(p[4], p[22]) ;  PIX_SORT(p[4], p[13]) ;  PIX_SORT(p[14], p[23]) ;
    PIX_SORT(p[5], p[23]) ;  PIX_SORT(p[5], p[14]) ;  PIX_SORT(p[15], p[24]) ;
    PIX_SORT(p[6], p[24]) ;  PIX_SORT(p[6], p[15]) ;  PIX_SORT(p[7], p[16]) ;
    PIX_SORT(p[7], p[19]) ;  PIX_SORT(p[13], p[21]) ; PIX_SORT(p[15], p[23]) ;
    PIX_SORT(p[7], p[13]) ;  PIX_SORT(p[7], p[15]) ;  PIX_SORT(p[1], p[9]) ;
    PIX_SORT(p[3], p[11]) ;  PIX_SORT(p[5], p[17]) ;  PIX_SORT(p[11], p[17]) ;
    PIX_SORT(p[9], p[17]) ;  PIX_SORT(p[4], p[10]) ;  PIX_SORT(p[6], p[12]) ;
    PIX_SORT(p[7], p[14]) ;  PIX_SORT(p[4], p[6]) ;   PIX_SORT(p[4], p[7]) ;
    PIX_SORT(p[12], p[14]) ; PIX_SORT(p[10], p[14]) ; PIX_SORT(p[6], p[7]) ;
    PIX_SORT(p[10], p[12]) ; PIX_SORT(p[6], p[10]) ;  PIX_SORT(p[6], p[17]) ;
    PIX_SORT(p[12], p[17]) ; PIX_SORT(p[7], p[17]) ;  PIX_SORT(p[7], p[10]) ;
    PIX_SORT(p[12], p[18]) ; PIX_SORT(p[7], p[12]) ;  PIX_SORT(p[10], p[18]) ;
    PIX_SORT(p[12], p[20]) ; PIX_SORT(p[10], p[20]) ; PIX_SORT(p[10], p[12]) ;

    return (p[12]);
}
/*----------------------------------------------------------------------------
   Function :   opt_med9()
   In       :   pointer to an array of 9 pixelvalues
   Out      :   a float
   Job      :   optimized search of the median of 9 pixelvalues
   Notice   :   in theory, cannot go faster without assumptions on the
                signal.
                Formula from:
                XILINX XCELL magazine, vol. 23 by John L. Smith
  
                The input array is modified in the process
                The result array is guaranteed to contain the median
                value
                in middle position, but other elements are NOT sorted.
 ---------------------------------------------------------------------------*/

float opt_med9(float * p)
{
    PIX_SORT(p[1], p[2]) ; PIX_SORT(p[4], p[5]) ; PIX_SORT(p[7], p[8]) ;
    PIX_SORT(p[0], p[1]) ; PIX_SORT(p[3], p[4]) ; PIX_SORT(p[6], p[7]) ;
    PIX_SORT(p[1], p[2]) ; PIX_SORT(p[4], p[5]) ; PIX_SORT(p[7], p[8]) ;
    PIX_SORT(p[0], p[3]) ; PIX_SORT(p[5], p[8]) ; PIX_SORT(p[4], p[7]) ;
    PIX_SORT(p[3], p[6]) ; PIX_SORT(p[1], p[4]) ; PIX_SORT(p[2], p[5]) ;
    PIX_SORT(p[4], p[7]) ; PIX_SORT(p[4], p[2]) ; PIX_SORT(p[6], p[4]) ;
    PIX_SORT(p[4], p[2]) ; return(p[4]) ;
}

kernel void SmallFilter(global float4* input,global float4* output){
    int x = get_global_id(0);
	int y = get_global_id(1);
	float depth[9];
	int index=x+y*WIDTH;
	float4 finalValue=input[index];

	if(x<1||x>=WIDTH-1||y<1||y>=HEIGHT-1){
		finalValue.w=0;
		output[index]=finalValue;
		return;
	}
	int size=0;
	int j,k,m;
	depth[0]=0;
	float mean=0;
	for(k=x-1; k <= x+1; k++) {
		for(m=y-1; m <= y+1; m++){
			float4 value=input[k+m*WIDTH];
			if(value.w >=CONFIDENCE_THRESHOLD) {
				mean+=depth[size] =value.z;
				size++;
			}
		}
	}
	if(size>0){
		mean/=size;
		float median=opt_med9(depth);
		float newDepth=(fabs(mean-median)<4.0f)?mean:median;
		finalValue.z=newDepth;
	}
	output[index]=finalValue;
}
kernel void LargeFilter(global float4* input,global float4* output){
    int x = get_global_id(0);
	int y = get_global_id(1);
	float depth[25];
	int index=x+y*WIDTH;
	float4 finalValue=input[index];

	if(x<2||x>=WIDTH-2||y<2||y>=HEIGHT-2){
		finalValue.w=0;
		output[index]=finalValue;
		return;
	}
	int size=0;
	int j,k,m;
	depth[0]=0;
	float mean=0;
	for(k=x-2; k <= x+2; k++) {
		for(m=y-2; m <= y+2; m++){
			float4 value=input[k+m*WIDTH];
			if(value.w >=CONFIDENCE_THRESHOLD) {
				mean+=depth[size] =value.z;
				size++;
			}
		}
	}
	if(size>0){
		mean/=size;
		float median=opt_med25(depth);
		float newDepth=(fabs(mean-median)<4.0f)?mean:median;
		finalValue.z=newDepth;
	}
	output[index]=finalValue;
}
kernel void ErodeFilter(global float4* input,global float4* output){
    int x = get_global_id(0);
	int y = get_global_id(1);
	int index=x+y*WIDTH;
	float4 finalValue=input[index];
	if(x<1||x>=WIDTH-1||y<1||y>=HEIGHT-1){
		output[index].w=0;
		return;
	}
	int size=0;
	int j,k,m;
	for(k=x-1; k <= x+1; k++) {
		for(m=y-1; m <= y+1; m++){
			float4 value=input[k+m*WIDTH];
			if(value.w >=CONFIDENCE_THRESHOLD) {
				size++;
			}
		}
	}
	if(size<9){
		finalValue.w=0;
		finalValue.z=MAX_DEPTH;
	}
	output[index]=finalValue;
}
kernel void DilateFilter(global float4* input,global float4* output){
    int x = get_global_id(0);
	int y = get_global_id(1);
	int index=x+y*WIDTH;
	float4 finalValue=input[index];
	if(x<1||x>=WIDTH-1||y<1||y>=HEIGHT-1){
		output[index].w=0;
		return;
	}
	int size=0;
	int j,k,m;
	if(finalValue.w<100){
		float mostConfident=0;
		for(k=x-1; k <= x+1; k++) {
			for(m=y-1; m <= y+1; m++){
				float4 value=input[k+m*WIDTH];
				if(value.w>mostConfident) {
					mostConfident=value.w;
					finalValue=value;
				}
			}
		}
	}
	output[index]=finalValue;
}
kernel void CopyToTemporalBuffer(global float4* motionBuffer,global float4* depthBuffer,global float4* depthTemporalBuffer, int index){
	const int gid = get_global_id(0);
	
	depthTemporalBuffer+=(HISTORY_SIZE*gid);
	float4 val=depthBuffer[gid];
	depthTemporalBuffer[index]=val;
	if(val.w<MIN_CONFIDENCE)val.w=0.0f; else val.w=1.0f;
	motionBuffer[gid]=val;
}
kernel void UpdateFilter(global float4* motionBuffer,global float4* depthBuffer,global float4* depthTemporalBuffer, int index){
	const int gid = get_global_id(0);
	
	depthTemporalBuffer+=(HISTORY_SIZE*gid);
	depthTemporalBuffer[index]=depthBuffer[gid];
	float sumw=0.0f;
	float4 value=(float4)(0.0f,0.0f,0.0f,0.0f);
	float minConfidence=1E10f;
	float maxConfidence=0.0f;
	float maxDepth=0.0f;
	float minDepth=1E10f;
	for(int i=0;i<HISTORY_SIZE;i++){
		float4 d=depthTemporalBuffer[i];
		minConfidence=min(minConfidence,d.w);
		maxConfidence=max(maxConfidence,d.w);
		minDepth=min(minDepth,d.z);
		maxDepth=max(maxDepth,d.z);
		sumw+=d.w;
		value+=d.w*d;
	}
	if(value.w>0.0f){
		value/=sumw;
	}
	motionBuffer[gid]=depthBuffer[gid];
	float motionZ=0.0f;
	float motionW=clamp(0.5f*(maxConfidence-minConfidence)/((float)HISTORY_SIZE*IR_RANGE_THRESH)-0.5f,0.0f,1.0f);
	float motion=max(motionW,motionZ);
	if(value.w>MIN_CONFIDENCE){
		value=mix(value,motionBuffer[gid],motion);
		value.w=depthBuffer[gid].w;
		depthBuffer[gid]=value;
	} else {
		motion=-1.0f;
	}
	motionBuffer[gid].w=motion;
}
kernel void CopyImage(global float4* depthData,global float2* uvData,__write_only image2d_t uvImage,write_only image2d_t depthImage)
{
    int i = get_global_id(0);
    int j = get_global_id(1);
    int2 coords = (int2)(i,j);   
    int index=i+j*get_global_size(0);
    float4 value=depthData[index];

	float2 uv=uvData[index];
	const sampler_t imageSampler = CLK_NORMALIZED_COORDS_FALSE | CLK_ADDRESS_CLAMP_TO_EDGE | CLK_FILTER_NEAREST;
	
	if(uv.x<0||uv.y<0||uv.y>1.0f||uv.x>1.0f||value.z<MIN_DEPTH||value.z>MAX_DEPTH){
		value.w=0.0f;
		value.z=MAX_DEPTH*4;
	}
	value.x=(i-WIDTH/2)*value.z/FOCAL_X; 
	value.y=(HEIGHT/2-j)*value.z/FOCAL_Y; 
	
	write_imagef(depthImage, coords,value);  
	
	depthData[index]=value;
	write_imagef(uvImage, coords,(float4)(uv.x,uv.y,0.0f,0.0f));  
		
}