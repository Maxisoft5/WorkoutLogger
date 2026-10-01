const fs = require('node:fs');
const path = require('node:path');
const assert = require('node:assert/strict');
const root = path.resolve(__dirname, '../WorkoutLogg');
const paletteXml = fs.readFileSync(path.join(root, 'Resources/Styles/ThemeColors.xaml'), 'utf8');
const colors = Object.fromEntries([...paletteXml.matchAll(/x:Key="(Theme\w+)">(#\w+)<\/Color>/g)].map(m => [m[1], m[2]]));
const luminance = hex => hex.slice(1).match(/../g).map(v => parseInt(v, 16) / 255)
    .map(v => v <= 0.04045 ? v / 12.92 : ((v + 0.055) / 1.055) ** 2.4)
    .reduce((sum, v, i) => sum + v * [0.2126, 0.7152, 0.0722][i], 0);
const contrast = (a, b) => (Math.max(luminance(a), luminance(b)) + 0.05) / (Math.min(luminance(a), luminance(b)) + 0.05);
for (const mode of ['Light', 'Dark']) {
    for (const [text, bg] of [['Text', 'Surface'], ['Secondary', 'Surface'], ['Muted', 'Surface'], ['Accent', 'AccentSoft'], ['OnHeroMuted', 'Hero']]) {
        const ratio = contrast(colors[`Theme${text}${mode}`], colors[`Theme${bg}${mode}`]);
        assert(ratio >= 4.5, `${mode}: ${text}/${bg} contrast ${ratio.toFixed(2)} is below 4.5:1`);
    }
}
const walk = dir => fs.readdirSync(dir, { withFileTypes: true }).flatMap(e =>
    ['bin', 'obj'].includes(e.name) ? [] : e.isDirectory() ? walk(path.join(dir, e.name)) : [path.join(dir, e.name)]);
let count = 0;
for (const file of walk(root).filter(f => f.endsWith('.xaml'))) {
    for (const [, key] of fs.readFileSync(file, 'utf8').matchAll(/StaticResource (Theme\w+)/g)) {
        assert(colors[key], `${file}: missing resource ${key}`);
        count++;
    }
}
console.log(`Theme contrast checks passed; ${count} resource references resolved.`);
