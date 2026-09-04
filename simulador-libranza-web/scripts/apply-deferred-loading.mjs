import { existsSync, readFileSync, writeFileSync } from 'node:fs';
import { join } from 'node:path';

const indexPath = join('dist', 'simulador-libranza-web', 'browser', 'index.html');

if (!existsSync(indexPath)) {
  console.error(`No se encontró ${indexPath}. Ejecuta ng build antes.`);
  process.exit(1);
}

let html = readFileSync(indexPath, 'utf8');
const scriptPattern = /<script src="([^"]+)" type="module"><\/script>/g;
const bundleSources = [...html.matchAll(scriptPattern)].map((match) => match[1]);

html = html.replace(scriptPattern, '<script src="$1" type="module" defer></script>');

if (bundleSources.length > 0) {
  const preloads = bundleSources
    .map((src) => `<link rel="modulepreload" href="${src}">`)
    .join('');
  if (!html.includes('rel="modulepreload"')) {
    html = html.replace('</head>', `${preloads}</head>`);
  }
}

writeFileSync(indexPath, html);
console.log(`Carga diferida aplicada a ${bundleSources.length} bundles: ${bundleSources.join(', ')}`);
