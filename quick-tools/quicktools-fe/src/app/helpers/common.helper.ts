export function maskString(
    value: string,
    visibleStart = 2,
    visibleEnd = 2,
    maskChar = '*',
): string {
    if (!value) return value;

    const length = value.length;

    // Không đủ ký tự để masking
    if (length <= visibleStart + visibleEnd) {
        return value;
    }

    const start = value.slice(0, visibleStart);
    const end = value.slice(length - visibleEnd);
    const maskedLength = length - visibleStart - visibleEnd;

    return start + maskChar.repeat(maskedLength) + end;
}
