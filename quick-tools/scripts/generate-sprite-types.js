const fs = require("fs");
const path = require("path");

const inputDir = path.join(__dirname, "../public/icons");
const outputDir = path.join(__dirname, "../src/app/shared/components/icon/types"); // tạo folder TS
const typeFile = path.join(outputDir, "icon-types.ts");

if (!fs.existsSync(outputDir)) fs.mkdirSync(outputDir, { recursive: true });

const files = fs.readdirSync(inputDir).filter(f => f.endsWith(".svg"));

const iconNames = files.map(f => `"${path.basename(f, ".svg")}"`).join(" | \n");

const content = `export type IconName = ${iconNames};\n`;

fs.writeFileSync(typeFile, content, "utf8");
console.log("✔ TypeScript type generated:", typeFile);
