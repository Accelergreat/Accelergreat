const fs = require('fs');
const path = require('path');
const marked = require('marked');

// Configuration
const INPUT_DIR = '.';
const OUTPUT_DIR = '_site';
const MARKDOWN_FILES = ['index.md', 'components.md', 'microservices.md'];

// Additional navigation items can be added here
const EXTERNAL_LINKS = [
  { title: 'GitHub', url: 'https://github.com/Accelergreat/Accelergreat', external: true }
];

// Ensure output directories exist
if (!fs.existsSync(OUTPUT_DIR)) {
  fs.mkdirSync(OUTPUT_DIR, { recursive: true });
}
if (!fs.existsSync(path.join(OUTPUT_DIR, 'styles'))) {
  fs.mkdirSync(path.join(OUTPUT_DIR, 'styles'), { recursive: true });
}

// Renderer to add IDs to headings
const renderer = new marked.Renderer();
const originalHeadingRenderer = renderer.heading;
renderer.heading = function(text, level) {
  const id = text.toLowerCase().replace(/[^\w\s-]/g, '').replace(/\s+/g, '-');
  return `<h${level} id="${id}">${text}</h${level}>`;
};

marked.setOptions({
  renderer: renderer,
  highlight: function(code, lang) {
    if (lang) {
      return `<pre><code class="language-${lang}">${escapeHtml(code)}</code></pre>`;
    }
    return `<pre><code>${escapeHtml(code)}</code></pre>`;
  },
  breaks: true,
  gfm: true
});

// HTML template
const htmlTemplate = (title, content, currentPage, toc) => `<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>${title} - Accelergreat Documentation</title>
    <link rel="icon" type="image/x-icon" href="/favicon.ico">
    <link rel="stylesheet" href="/styles/main.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/themes/prism-tomorrow.min.css">
</head>
<body>
    <header>
        <nav class="navbar">
            <div class="container">
                <div class="navbar-brand">
                    <img src="/accelergreat_icon.png" alt="Accelergreat" class="logo">
                    <span class="brand-text">Accelergreat</span>
                </div>
                <ul class="navbar-nav">
                    <li class="nav-item">
                        <a href="/" class="nav-link ${currentPage === 'index' ? 'active' : ''}">Home</a>
                    </li>
                    <li class="nav-item">
                        <a href="/components.html" class="nav-link ${currentPage === 'components' ? 'active' : ''}">Components</a>
                    </li>
                    <li class="nav-item">
                        <a href="/microservices.html" class="nav-link ${currentPage === 'microservices' ? 'active' : ''}">Microservices</a>
                    </li>
                    ${EXTERNAL_LINKS.map(link => `
                    <li class="nav-item">
                        <a href="${link.url}" class="nav-link ${link.external ? 'external' : ''}" ${link.external ? 'target="_blank"' : ''}>${link.title}</a>
                    </li>`).join('')}
                </ul>
            </div>
        </nav>
    </header>
    
    <main class="main-wrapper">
        <div class="container">
            <div class="content-wrapper">
                <div class="content">
                    ${content}
                </div>
                ${toc ? `<aside class="sidebar-right">${toc}</aside>` : ''}
            </div>
        </div>
    </main>
    
    <footer>
        <div class="container">
            <p>&copy; ${new Date().getFullYear()} Accelergreat. Built with ❤️ for fast integration testing.</p>
        </div>
    </footer>
    
    <script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/prism.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/components/prism-csharp.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/components/prism-bash.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/components/prism-json.min.js"></script>
    
    <script>
    // Table of Contents scroll spy
    document.addEventListener('DOMContentLoaded', function() {
        const tocLinks = document.querySelectorAll('.toc-link');
        const headings = document.querySelectorAll('h2[id], h3[id], h4[id]');
        
        function updateActiveLink() {
            let current = '';
            const scrollPosition = window.scrollY + 100;
            
            headings.forEach(heading => {
                const rect = heading.getBoundingClientRect();
                if (rect.top + window.scrollY <= scrollPosition) {
                    current = heading.id;
                }
            });
            
            tocLinks.forEach(link => {
                link.classList.remove('active');
                if (link.getAttribute('href') === '#' + current) {
                    link.classList.add('active');
                }
            });
        }
        
        // Smooth scrolling for TOC links
        tocLinks.forEach(link => {
            link.addEventListener('click', function(e) {
                e.preventDefault();
                const targetId = this.getAttribute('href').substring(1);
                const targetElement = document.getElementById(targetId);
                if (targetElement) {
                    window.scrollTo({
                        top: targetElement.offsetTop - 80,
                        behavior: 'smooth'
                    });
                }
            });
        });
        
        window.addEventListener('scroll', updateActiveLink);
        updateActiveLink(); // Initial call
    });
    </script>
</body>
</html>`;



function escapeHtml(text) {
  const map = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#039;'
  };
  return text.replace(/[&<>"']/g, m => map[m]);
}

// Extract headings for table of contents
function extractHeadings(content) {
  const headings = [];
  const headingRegex = /^(#{2,4})\s+(.+)$/gm;
  let match;
  
  while ((match = headingRegex.exec(content)) !== null) {
    const level = match[1].length;
    const text = match[2].trim();
    const id = text.toLowerCase().replace(/[^\w\s-]/g, '').replace(/\s+/g, '-');
    headings.push({ level, text, id });
  }
  
  return headings;
}

// Generate TOC HTML
function generateTOC(headings) {
  if (headings.length === 0) return '';
  
  let html = '<nav class="toc">\n<h3>In This Article</h3>\n<ul class="toc-list">\n';
  
  headings.forEach(heading => {
    const indent = heading.level - 2;
    html += `<li class="toc-item toc-level-${heading.level}" style="padding-left: ${indent * 1}rem;">
      <a href="#${heading.id}" class="toc-link">${heading.text}</a>
    </li>\n`;
  });
  
  html += '</ul>\n</nav>';
  return html;
}

// Process markdown files
MARKDOWN_FILES.forEach(file => {
  const inputPath = path.join(INPUT_DIR, file);
  const outputFile = file.replace('.md', '.html');
  const outputPath = path.join(OUTPUT_DIR, outputFile);
  
  if (fs.existsSync(inputPath)) {
    console.log(`Processing ${file}...`);
    
    // Read markdown content
    let content = fs.readFileSync(inputPath, 'utf8');
    
    // Remove YAML frontmatter if present
    content = content.replace(/^---[\s\S]*?---\n*/m, '');
    
    // Extract headings for TOC
    const headings = extractHeadings(content);
    const toc = generateTOC(headings);
    
    // Convert markdown to HTML
    const htmlContent = marked.marked ? marked.marked(content) : marked(content);
    
    // Extract title from first H1
    const titleMatch = content.match(/^#\s+(.+)$/m);
    const title = titleMatch ? titleMatch[1] : path.basename(file, '.md');
    
    // Generate full HTML
    const currentPage = path.basename(file, '.md');
    const fullHtml = htmlTemplate(title, htmlContent, currentPage, toc);
    
    // Write output
    fs.writeFileSync(outputPath, fullHtml);
    console.log(`Created ${outputPath}`);
  }
});

// Write CSS file
console.log('Creating CSS file...');
const cssContent = `/* Accelergreat Documentation Styles */

:root {
  /* Color palette */
  --primary-color: #007ACC;
  --primary-dark: #005a9e;
  --primary-light: #2d9cdb;
  --accent-color: #ffc107;
  --background: #ffffff;
  --surface: #f8f9fa;
  --text-primary: #212529;
  --text-secondary: #6c757d;
  --border-color: #dee2e6;
  --code-background: #f5f5f5;
  --link-color: var(--primary-color);
  --link-hover: var(--primary-dark);
  
  /* Spacing */
  --spacing-xs: 0.25rem;
  --spacing-sm: 0.5rem;
  --spacing-md: 1rem;
  --spacing-lg: 1.5rem;
  --spacing-xl: 2rem;
  --spacing-xxl: 3rem;
  
  /* Typography */
  --font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
  --font-family-mono: "SFMono-Regular", Consolas, "Liberation Mono", Menlo, Courier, monospace;
  --font-size-base: 16px;
  --line-height-base: 1.6;
}

/* Dark mode */
@media (prefers-color-scheme: dark) {
  :root {
    --background: #1a1a1a;
    --surface: #2d2d2d;
    --text-primary: #e0e0e0;
    --text-secondary: #a0a0a0;
    --border-color: #404040;
    --code-background: #2d2d2d;
  }
}

/* Reset and base styles */
* {
  box-sizing: border-box;
}

body {
  margin: 0;
  font-family: var(--font-family);
  font-size: var(--font-size-base);
  line-height: var(--line-height-base);
  color: var(--text-primary);
  background-color: var(--background);
}

/* Container */
.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
}

/* Header and Navigation */
header {
  background-color: var(--surface);
  border-bottom: 1px solid var(--border-color);
  position: sticky;
  top: 0;
  z-index: 1000;
}

.navbar {
  padding: var(--spacing-md) 0;
}

.navbar .container {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.navbar-brand {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  text-decoration: none;
  color: var(--text-primary);
}

.navbar-brand .logo {
  height: 40px;
  width: 40px;
}

.brand-text {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--primary-color);
}

.navbar-nav {
  display: flex;
  list-style: none;
  margin: 0;
  padding: 0;
  gap: var(--spacing-lg);
}

.nav-item {
  margin: 0;
}

.nav-link {
  color: var(--text-primary);
  text-decoration: none;
  padding: var(--spacing-sm) 0;
  border-bottom: 2px solid transparent;
  transition: all 0.3s ease;
}

.nav-link:hover {
  color: var(--primary-color);
  border-bottom-color: var(--primary-color);
}

.nav-link.active {
  color: var(--primary-color);
  border-bottom-color: var(--primary-color);
  font-weight: 600;
}

.nav-link.external::after {
  content: "↗";
  margin-left: var(--spacing-xs);
  font-size: 0.8em;
}

/* Main content */
.main-wrapper {
  min-height: calc(100vh - 200px);
  padding: var(--spacing-xxl) 0;
}

.content-wrapper {
  display: flex;
  gap: var(--spacing-xxl);
  align-items: flex-start;
}

.content {
  flex: 1;
  max-width: 900px;
  min-width: 0;
}

/* Table of Contents Sidebar */
.sidebar-right {
  position: sticky;
  top: calc(60px + var(--spacing-xl));
  width: 250px;
  max-height: calc(100vh - 120px);
  overflow-y: auto;
  padding-left: var(--spacing-lg);
  border-left: 1px solid var(--border-color);
}

.toc {
  padding: 0;
}

.toc h3 {
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: var(--text-secondary);
  margin-bottom: var(--spacing-md);
  font-weight: 600;
}

.toc-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.toc-item {
  margin-bottom: var(--spacing-xs);
}

.toc-link {
  display: block;
  padding: var(--spacing-xs) 0;
  color: var(--text-secondary);
  text-decoration: none;
  font-size: 0.875rem;
  border-left: 2px solid transparent;
  padding-left: var(--spacing-sm);
  margin-left: -2px;
  transition: all 0.2s ease;
}

.toc-link:hover {
  color: var(--text-primary);
  text-decoration: none;
}

.toc-link.active {
  color: var(--primary-color);
  border-left-color: var(--primary-color);
  font-weight: 600;
}

.toc-level-3 {
  font-size: 0.813rem;
}

.toc-level-4 {
  font-size: 0.75rem;
}

/* Hide TOC on smaller screens */
@media (max-width: 1200px) {
  .sidebar-right {
    display: none;
  }
  
  .content {
    max-width: 100%;
  }
}

/* Typography */
h1, h2, h3, h4, h5, h6 {
  margin-top: var(--spacing-xl);
  margin-bottom: var(--spacing-md);
  font-weight: 600;
  line-height: 1.3;
  color: var(--text-primary);
}

h1 {
  font-size: 2.5rem;
  border-bottom: 2px solid var(--border-color);
  padding-bottom: var(--spacing-md);
  margin-top: 0;
}

h2 {
  font-size: 2rem;
  margin-top: var(--spacing-xxl);
}

h3 {
  font-size: 1.5rem;
}

h4 {
  font-size: 1.25rem;
}

p {
  margin-bottom: var(--spacing-md);
}

/* Links */
a {
  color: var(--link-color);
  text-decoration: none;
}

a:hover {
  color: var(--link-hover);
  text-decoration: underline;
}

/* Lists */
ul, ol {
  margin-bottom: var(--spacing-md);
  padding-left: var(--spacing-xl);
}

li {
  margin-bottom: var(--spacing-xs);
}

/* Code blocks */
pre {
  background-color: var(--code-background);
  border: 1px solid var(--border-color);
  border-radius: 4px;
  padding: var(--spacing-md);
  overflow-x: auto;
  margin-bottom: var(--spacing-md);
}

code {
  font-family: var(--font-family-mono);
  font-size: 0.875em;
}

/* Inline code */
p code, li code {
  background-color: var(--code-background);
  padding: 0.2em 0.4em;
  border-radius: 3px;
  font-size: 0.875em;
}

/* Blockquotes */
blockquote {
  border-left: 4px solid var(--primary-color);
  padding-left: var(--spacing-md);
  margin: var(--spacing-md) 0;
  color: var(--text-secondary);
}

/* Tables */
table {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: var(--spacing-md);
}

th, td {
  padding: var(--spacing-sm) var(--spacing-md);
  text-align: left;
  border-bottom: 1px solid var(--border-color);
}

th {
  background-color: var(--surface);
  font-weight: 600;
}

/* Footer */
footer {
  background-color: var(--surface);
  border-top: 1px solid var(--border-color);
  padding: var(--spacing-xl) 0;
  text-align: center;
  color: var(--text-secondary);
}

/* Responsive */
@media (max-width: 768px) {
  .navbar .container {
    flex-direction: column;
    gap: var(--spacing-md);
  }
  
  .navbar-nav {
    flex-wrap: wrap;
    justify-content: center;
    gap: var(--spacing-md);
  }
  
  h1 {
    font-size: 2rem;
  }
  
  h2 {
    font-size: 1.5rem;
  }
  
  .content {
    padding: 0 var(--spacing-md);
  }
}

/* Utility classes */
.text-muted {
  color: var(--text-secondary);
}

.mt-0 { margin-top: 0; }
.mb-0 { margin-bottom: 0; }
.mt-1 { margin-top: var(--spacing-sm); }
.mb-1 { margin-bottom: var(--spacing-sm); }
.mt-2 { margin-top: var(--spacing-md); }
.mb-2 { margin-bottom: var(--spacing-md); }
.mt-3 { margin-top: var(--spacing-lg); }
.mb-3 { margin-bottom: var(--spacing-lg); }

/* Emoji Enhancement */
h1:has(⚡) {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

h2:has(🚀), h2:has(🎯) {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}`;

fs.writeFileSync(path.join(OUTPUT_DIR, 'styles', 'main.css'), cssContent);
console.log('CSS file created');

console.log('\nBuilding documentation site completed!');