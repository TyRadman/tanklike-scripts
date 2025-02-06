#ifndef GRASS_TRAMPLE_INCLUDED
#define GRASS_TRAMPLE_INCLUDED

float _NumGrassTramplePositions;

float4 _GrassTramplePositions[8];

void CalculateTrample_float(float3 WorldPosition, float MaxPosition, float Falloff, float PushAwayStrength, float PushDownStrength, out float3 Offset, out float WindMultiplier)
{
    Offset = 0;
    WindMultiplier = 1;
    
#ifndef SHADERGRAPH_PREVIEW
    for (int i = 0; i < _NumGrassTramplePositions; i++)
    {
        float3 objectPositionWS = _GrassTramplePositions[i].xyz;
        float distanceVector = WorldPosition - objectPositionWS;
        float distance = length(distanceVector);
        
        float strength = 1 - pow(saturate(distance / MaxPosition), Falloff);
        
        float3 xzDistance = distanceVector;
        xzDistance.y = 0;
        float3 pushAwayOffset = normalize(xzDistance) * strength * PushAwayStrength;
        
        float3 squishOffset = float3(0, -strength * PushDownStrength, 0);
        
        Offset += pushAwayOffset + squishOffset;
        
        WindMultiplier = min(WindMultiplier, 1 - strength);
    }
#endif
}

#endif