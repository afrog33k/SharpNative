// Written in the D programming language.

/**

$(BOOKTABLE ,
$(TR $(TH Category) $(TH Functions))
$(TR $(TDNW Template API) $(TD $(MYREF XXHash)))
$(TR $(TDNW Helpers) $(TD $(MYREF xxhashOf)))
)

 * xxhash: Extremely fast non-cryptographic hash algorithm
 *
 * Example:
 * -----
 * uint hashed = xxhashOf(cast(ubyte[])"D-man is so cute");
 * assert(hashed == 3228048616);
 * -----
 *
 * See_Also:
 *  $(LINK2 https://code.google.com/p/xxhash/, xxhash - Extremely fast non-cryptographic hash algorithm)
 *
 * Copyright: Copyright Masahiro Nakagawa 2014-.
 * License:   <a href="http://www.boost.org/LICENSE_1_0.txt">Boost License 1.0</a>.
 * Authors:   Masahiro Nakagawa
 */
module xxhash;

import std.bitmanip : swapEndian;

@trusted pure nothrow
uint xxhashOf(in char[] source, uint seed = 0)
{
    auto srcPtr = cast(const(char)*)source.ptr;
    auto srcEnd = cast(const(char)*)(source.ptr + source.length);
    uint result = void;

    if (source.length >= 16) {
        auto limit = srcEnd - 4;
        uint v1 = seed + Prime32_1 + Prime32_2;
        uint v2 = seed + Prime32_2;
        uint v3 = seed;
        uint v4 = seed - Prime32_1;

        do {
            mixin(UpdateValuesRound);
        } while (srcPtr <= limit);

        result = rotateLeft(v1, 1) + rotateLeft(v2, 7) + rotateLeft(v3, 12) + rotateLeft(v4, 18);
    } else {
        result = seed + Prime32_5;
    }

    result += source.length;

    while (srcPtr <= srcEnd - 1) {
        result += loadUint(srcPtr) * Prime32_3;
        result = rotateLeft(result, 17) * Prime32_4;
        srcPtr++;
    }

    auto ptr = cast(const(ubyte)*)srcPtr;
    auto end = cast(const(ubyte)*)srcEnd;

    mixin(FinishRound);

    return result;
}

@trusted pure nothrow
uint xxhashOf(in wchar[] source, uint seed = 0)
{
    auto srcPtr = cast(const(wchar)*)source.ptr;
    auto srcEnd = cast(const(wchar)*)(source.ptr + source.length);
    uint result = void;

    if (source.length >= 16) {
        auto limit = srcEnd - 4;
        uint v1 = seed + Prime32_1 + Prime32_2;
        uint v2 = seed + Prime32_2;
        uint v3 = seed;
        uint v4 = seed - Prime32_1;

        do {
            mixin(UpdateValuesRound);
        } while (srcPtr <= limit);

        result = rotateLeft(v1, 1) + rotateLeft(v2, 7) + rotateLeft(v3, 12) + rotateLeft(v4, 18);
    } else {
        result = seed + Prime32_5;
    }

    result += source.length;

    while (srcPtr <= srcEnd - 1) {
        result += loadUint(srcPtr) * Prime32_3;
        result = rotateLeft(result, 17) * Prime32_4;
        srcPtr++;
    }

    auto ptr = cast(const(wchar)*)srcPtr;
    auto end = cast(const(wchar)*)srcEnd;

    mixin(FinishRound);

    return result;
}

/**
 * Computes xxhash hashes of arbitrary data.
 *
 * Returns:
 *  4 byte hash value.
 */
@trusted pure nothrow
uint xxhashOf(in ubyte[] source, uint seed = 0)
{
    auto srcPtr = cast(const(uint)*)source.ptr;
    auto srcEnd = cast(const(uint)*)(source.ptr + source.length);
    uint result = void;

    if (source.length >= 16) {
        auto limit = srcEnd - 4;
        uint v1 = seed + Prime32_1 + Prime32_2;
        uint v2 = seed + Prime32_2;
        uint v3 = seed;
        uint v4 = seed - Prime32_1;

        do {
            mixin(UpdateValuesRound);
        } while (srcPtr <= limit);

        result = rotateLeft(v1, 1) + rotateLeft(v2, 7) + rotateLeft(v3, 12) + rotateLeft(v4, 18);
    } else {
        result = seed + Prime32_5;
    }

    result += source.length;

    while (srcPtr <= srcEnd - 1) {
        result += loadUint(srcPtr) * Prime32_3;
        result = rotateLeft(result, 17) * Prime32_4;
        srcPtr++;
    }

    auto ptr = cast(const(ubyte)*)srcPtr;
    auto end = cast(const(ubyte)*)srcEnd;

    mixin(FinishRound);

    return result;
}

/**
 * xxhash object implements std.digest like API for supporting streaming update.
 *
 * Example:
 * -----
 * XXHash xh;
 *
 * xh.start();
 * foreach (chunk; chunks(cast(ubyte[])"D-man is so cute!", 2))
 *     xh.put(chunk);
 * auto hashed = xh.finish();
 * -----
 */
struct XXHash
{
  private:
    uint _seed = 0;
    uint _v1, _v2, _v3, _v4;
    ubyte[16] _memory;
    size_t _memorySize;
    ulong _totalLength;

  public:
    @safe pure nothrow
    {
        /**
         * Constructs XXHash with seed.
         */
        this(uint seed)
        {
            _seed = seed;
        }

        /**
         * Used to (re)initialize the XXHash.
         */
        void start()
        {
            _v1 = _seed + Prime32_1 + Prime32_2;
            _v2 = _seed + Prime32_2;
            _v3 = _seed;
            _v4 = _seed - Prime32_1;
            _memorySize = 0;
            _totalLength = 0;
        }

        /**
         * Use this to feed the hash with data.
         * Also implements the $(XREF range, OutputRange) interface for $(D ubyte) and $(D const(ubyte)[]).
         */
        @trusted
        void put(scope const(ubyte)[] data...)
        {
            auto ptr = data.ptr;
            auto end = ptr + data.length;

            _totalLength += data.length;

            if (_memorySize + data.length < 16) {
                _memory[_memorySize.._memorySize + data.length] = data;
                _memorySize += data.length;
                return;
            }

            if (_memorySize > 0) {
                auto sliceSize = 16 - _memorySize;
                _memory[_memorySize.._memorySize + sliceSize] = data[0..sliceSize];

                auto v1 = _v1, v2 = _v2, v3 = _v3, v4 = _v4;
                auto srcPtr = cast(const(uint)*)_memory.ptr;

                mixin(UpdateValuesRound);

                ptr += sliceSize;
                _memorySize = 0;
                _v1 = v1; _v2 = v2; _v3 = v3, _v4 = v4;
            }

            if (ptr <= end - 16) {
                auto srcPtr = cast(const(uint)*)ptr;
                auto limit = cast(const(uint)*)(end - 16);
                auto v1 = _v1, v2 = _v2, v3 = _v3, v4 = _v4;

                do {
                    mixin(UpdateValuesRound);
                } while (srcPtr <= limit);

                ptr = cast(const(ubyte)*)srcPtr;
                _v1 = v1; _v2 = v2; _v3 = v3, _v4 = v4;
            }

            if (ptr < end) {
                auto remain = end - ptr;
                _memory[0..remain] = ptr[0..remain];
                _memorySize = remain;
            }
        }

        /**
         * Returns the finished XXHash hash.
         * This also calls $(LREF start) to reset the internal state.
         */
        @trusted
        uint finish()
        {
            auto ptr = _memory.ptr;
            auto end = ptr + _memorySize;
            uint result = void;

            if (_totalLength >= 16)
                result = rotateLeft(_v1, 1) + rotateLeft(_v2, 7) + rotateLeft(_v3, 12) + rotateLeft(_v4, 18);
            else
                result = _seed + Prime32_5;

            result += cast(uint)_totalLength;

            while (ptr <= end - 4) {
                result += loadUint(cast(const(uint)*)ptr) * Prime32_3;
                result = rotateLeft(result, 17) * Prime32_4;
                ptr += 4;
            }

            mixin(FinishRound);

            start();

            return result;
        }
    }
}

private:

enum UpdateValuesRound = "
    v1 += loadUint(srcPtr) * Prime32_2; v1 = rotateLeft(v1, 13);
    v1 *= Prime32_1; srcPtr++;
    v2 += loadUint(srcPtr) * Prime32_2; v2 = rotateLeft(v2, 13);
    v2 *= Prime32_1; srcPtr++;
    v3 += loadUint(srcPtr) * Prime32_2; v3 = rotateLeft(v3, 13);
    v3 *= Prime32_1; srcPtr++;
    v4 += loadUint(srcPtr) * Prime32_2; v4 = rotateLeft(v4, 13);
    v4 *= Prime32_1; srcPtr++;
";

enum FinishRound = "
    while (ptr < end) {
        result += *ptr * Prime32_5;
        result = rotateLeft(result, 11) * Prime32_1 ;
        ptr++;
    }

    result ^= result >> 15;
    result *= Prime32_2;
    result ^= result >> 13;
    result *= Prime32_3;
    result ^= result >> 16;
";

enum Prime32_1 = 2654435761U;
enum Prime32_2 = 2246822519U;
enum Prime32_3 = 3266489917U;
enum Prime32_4 = 668265263U;
enum Prime32_5 = 374761393U;

@safe pure nothrow
uint rotateLeft(in uint x, in uint n)
{
    return (x << n) | (x >> (32 - n));
}

@safe pure nothrow
uint loadUint(in uint* source)
{
    version (LittleEndian)
        return *source;
    else
        return swapEndian(*source);
}

@safe pure nothrow
uint loadUint(in char* source)
{
    version (LittleEndian)
        return *source;
    else
        return swapEndian(*source);
}

@safe pure nothrow
uint loadUint(in wchar* source)
{
    version (LittleEndian)
        return *source;
    else
        return swapEndian(*source);
}

unittest
{
    import std.range : chunks;

    void runTests(ubyte[] sentence, uint seed, uint expected)
    {
        assert(xxhashOf(sentence, seed) == expected, "xxhashOf failed");

        XXHash xh = XXHash(seed);

        xh.start();
        xh.put(sentence);
        assert(xh.finish() == expected, "XXHash once failed");

        xh.start();
        foreach (chunk; chunks(sentence, 1))
            xh.put(chunk);
        assert(xh.finish() == expected, "XXHash streaming failed");
    }

    enum PRIME = 2654435761U;
    uint random = PRIME;
    ubyte[101] sanityBuffer;

    foreach (i; 0..sanityBuffer.length) {
        sanityBuffer[i] = cast(ubyte)(random >> 24);
        random *= random;
    }

    // test cases from xxhash's bench.c
    auto length = sanityBuffer.length;
    runTests(sanityBuffer[0..1],      0, 0xB85CBEE5);
    runTests(sanityBuffer[0..1],  PRIME, 0xD5845D64);
    runTests(sanityBuffer[0..14],     0, 0xE5AA0AB4);
    runTests(sanityBuffer[0..14], PRIME, 0x4481951D);
    runTests(sanityBuffer,            0, 0x1F1AA412);
    runTests(sanityBuffer,        PRIME, 0x498EC8E2);
}
