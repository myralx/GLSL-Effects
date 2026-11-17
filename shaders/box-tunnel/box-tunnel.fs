#version 330

out vec4 color;
uniform vec2 resolution;
uniform float time;

mat2 rot(float a) // Build a 2x2 matrix
{
    float c = cos(a), s = sin(a); // cos and sine
    return mat2(c, s, -s, c); // Builds a rotation matrix
}

float boxDist(vec3 point, vec3 halfSize) // Returns distance from ray to box surface
// Negative inside the box, zero on surface and positive outside of it.
{
    // How far point is past box edge on each axis
    vec3 offset = abs(point) - halfSize; 
    float largestOffset = max(offset.x, max(offset.y, offset.z));

    if (largestOffset > 0.0) // positive offset means point is outside
    {
        // Combine positive gaps into straight line distance.
        return length(max(offset, 0.0));
    }

    // Offset closest to zero is the nearest face
    return largestOffset;
}

float box(vec3 pos, float scale)
{
    // Scale the point and measure its distance
    float distanceToBox = boxDist(pos * scale, vec3(0.4, 0.4, 0.1));

    // Make the distance positive inside and reduce its scale.
    return -distanceToBox / 1.5;
}

float boxSet(vec3 pos, float boxTime)
{
    float pulse = sin(boxTime * 0.4); // How fast the light pulses
    float shift = pulse * 2.5; // How far each box is from centre
    float scale = 2.0 - abs(pulse) * 1.5; // Size of boxes
    mat2 rotation = rot(0.8); // Completes the earlier rotation matrix

    // Rotates the point and the two offsets into the same space
    vec3 rotatedPos = vec3(pos.xy * rotation, pos.z);
    vec3 yOffset = vec3(vec2(0.0, shift) * rotation, 0.0);
    vec3 xOffset = vec3(vec2(shift, 0.0) * rotation, 0.0);

    // Add and subtract each offset to get a pair of boxes
    float vertical = max(box(rotatedPos + yOffset, scale),box(rotatedPos - yOffset, scale));
    float horizontal = max(box(rotatedPos + xOffset, scale),box(rotatedPos - xOffset, scale));

    // Combine a rotated and an unrotated box at the centre.
    float middle = max(box(rotatedPos, 0.5), box(pos, 0.5)) * 6.0;

    return max(max(vertical, horizontal), middle);
}

void main()
{
    // Centre the pixel coordinates and keep the boxes in proportion
    vec2 p = (gl_FragCoord.xy * 2.0 - resolution)
           / min(resolution.x, resolution.y);

    vec3 camera = vec3(0.0, -0.2, time * 4.0);
    vec3 ray = normalize(vec3(p, 1.5));
    ray.xy = ray.xy * rot(sin(time * 0.03) * 5.0);
    ray.yz = ray.yz * rot(sin(time * 0.05) * 0.2);

    float t = 0.1;
    float glow = 0.0;

    for (int i = 0; i < 99; i++)
    {
        // Repeat the same box arrangement every four units in each direction
        vec3 pos = mod(camera + ray * t - 2.0, 4.0) - 2.0;
        float boxTime = time - float(i) * 0.01;
        float distanceToSurface = max(abs(boxSet(pos, boxTime)), 0.01);

        glow += exp(-distanceToSurface * 23.0);
        t += distanceToSurface * 0.55;
    }

    float pulse = sin(time);
    vec3 rgb = vec3(glow * 0.02);
    rgb += vec3(0.0, 0.2 * abs(pulse), 0.5 + pulse * 0.2);

    float alpha = clamp(1.0 - t * (0.02 + 0.02 * pulse), 0.0, 1.0);
    color = vec4(rgb, alpha);
}