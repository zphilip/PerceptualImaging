/*******************************************************************************

INTEL CORPORATION PROPRIETARY INFORMATION
This software is supplied under the terms of a license agreement or nondisclosure
agreement with Intel Corporation and may not be copied or disclosed except in
accordance with the terms of that agreement
Copyright(c) 2012 Intel Corporation. All Rights Reserved.

@Author {Blake C. Lucas (img.science@gmail.com)}
*******************************************************************************/

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
			if(value.w >=MIN_IR) {
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
			if(value.w >=MIN_IR) {
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
			if(value.w >=MIN_IR) {
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
kernel void CopyToTemporalBuffer(int index,global float4* motionBuffer,global float4* depthBuffer,global float4* depthTemporalBuffer){
	const int gid = get_global_id(0);
	
	depthTemporalBuffer+=(HISTORY_SIZE*gid);
	float4 val=depthBuffer[gid];
	depthTemporalBuffer[index]=val;
	if(val.w<MIN_CONFIDENCE)val.w=0.0f; else val.w=1.0f;
	motionBuffer[gid]=val;
}
kernel void UpdateFilter(int index,global float4* motionBuffer,global float4* depthBuffer,global float4* depthTemporalBuffer){
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

constant float m_RotationMatrix[]={
		0.99996847f,
		0.0012344177f,
		-0.0078440439f,
		0.0012864748f,
		-0.99997717f,
		0.0066349362f,
		0.0078356741f,
		0.0066448180f,
		0.99994725f};
constant float m_ColorDistortions[]={
		0.022575200f,
		-0.16266800f,
		0.18613800f,	
		0.00000000f,	
		0.00000000f};
		
constant float m_DepthDistortions[]={
		-0.17010300f,
		0.14406399f,
		-0.047699399f,
		0.00000000f,
		0.00000000f};

constant float m_TransMatrix[]={24.492104f,0.50799217f,-0.86258771};//y component is positive because we do not need to negate pos3d.y
#define COLOR_FOCAL_X 583.07898f
#define COLOR_FOCAL_Y 596.20300f
#define COLOR_CENTER_X 319.00000f
#define COLOR_CENTER_Y 239.00000f

inline float2 MapDepthToColor(float4 pos3D){
	float4 pos3DColor;
    
	
	float2 tex2d=(float2)(-1,-1);
	
	if(pos3D.z>MIN_DEPTH&&pos3D.z<MAX_DEPTH&&pos3D.w>=MIN_IR){
		pos3D.w=1.0f;
		pos3D.x=-pos3D.x;
	
		pos3D.x-=m_TransMatrix[0];
		pos3D.y-=m_TransMatrix[1];
		pos3D.z-=m_TransMatrix[2];
   
		pos3DColor.x = pos3D.x*m_RotationMatrix[0]+pos3D.y*m_RotationMatrix[1]+pos3D.z*m_RotationMatrix[2];
		pos3DColor.y = pos3D.x*m_RotationMatrix[3]+pos3D.y*m_RotationMatrix[4]+pos3D.z*m_RotationMatrix[5];
		pos3DColor.z = pos3D.x*m_RotationMatrix[6]+pos3D.y*m_RotationMatrix[7]+pos3D.z*m_RotationMatrix[8];
		float fy = pos3DColor.y/pos3DColor.z;
        float fx = pos3DColor.x/pos3DColor.z;
        float fr2= fy*fy+fx*fx;
        float fDistC = 1+m_ColorDistortions[0]*fr2+m_ColorDistortions[1]*fr2*fr2+m_ColorDistortions[2]*fr2*fr2*fr2;    
        tex2d.y = (fy*fDistC*COLOR_FOCAL_Y + COLOR_CENTER_Y)/(float)COLOR_HEIGHT;
        tex2d.x = (COLOR_CENTER_X-fx*fDistC*COLOR_FOCAL_X)/(float)COLOR_WIDTH;
	}
	return tex2d;
}
kernel void CopyImage(global float4* depthData,global float2* uvData,__write_only image2d_t uvImage,write_only image2d_t depthImage)
{
    int i = get_global_id(0);
    int j = get_global_id(1);
    int2 coords = (int2)(i,j);   
    int index=i+j*get_global_size(0);
    float4 value=depthData[index];

	const sampler_t imageSampler = CLK_NORMALIZED_COORDS_FALSE | CLK_ADDRESS_CLAMP_TO_EDGE | CLK_FILTER_NEAREST;
	
	if(value.z<MIN_DEPTH||value.z>MAX_DEPTH){
		value.w=0.0f;
		value.z=MAX_DEPTH*4;
	}
	value.x=(i-CENTER_X)*value.z/FOCAL_X; 
	value.y=(CENTER_Y-j)*value.z/FOCAL_Y; 
	
	float2 uv=MapDepthToColor(value);
	write_imagef(depthImage, coords,value);  
	
	depthData[index]=value;
	uvData[index]=uv;
	write_imagef(uvImage, coords,(float4)(uv.x,uv.y,0,0));  
		
}

