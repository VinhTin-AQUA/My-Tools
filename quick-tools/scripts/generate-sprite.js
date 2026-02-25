const fs = require('fs');
const path = require('path');

const spriteFileName = 'sprite.svg';

const inputDir = path.join(__dirname, '../public/icons');
const outputDir = path.join(__dirname, '../public/icons');
const outputFile = path.join(outputDir, spriteFileName);

// Tạo thư mục output nếu chưa có
if (!fs.existsSync(outputDir)) {
    fs.mkdirSync(outputDir, { recursive: true });
}

let symbols = '';

const files = fs.readdirSync(inputDir).filter((f) => f.endsWith('.svg') && f !== spriteFileName);

files.forEach((file) => {
    const filePath = path.join(inputDir, file);
    let svg = fs.readFileSync(filePath, 'utf8');

    // --- Làm sạch SVG ---
    svg = svg
        .replace(/<\?xml.*?\?>/g, '') // remove xml tag
        .replace(/<!DOCTYPE.*?>/g, '') // remove doctype
        .replace(/<svg[^>]*>/g, '') // remove svg wrapper
        .replace(/<\/svg>/g, '') // remove svg wrapper
        .replace(/fill=".*?"/g, 'fill="currentColor"') // remove fill
        .replace(/stroke=".*?"/g, 'stroke="currentColor"'); // remove stroke

    const id = path.basename(file, '.svg');

    // Thêm symbol
    symbols += `
  <symbol id="${id}" viewBox="0 0 24 24">
    ${svg.trim()}
  </symbol>
  `;
});

// Kết hợp thành file sprite
const sprite = `
<svg xmlns="http://www.w3.org/2000/svg">
${symbols}
</svg>
`;

fs.writeFileSync(outputFile, sprite, 'utf8');

console.log('✔ SVG Sprite generated:', outputFile);
