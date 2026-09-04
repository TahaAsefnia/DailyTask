const API = 'http://localhost:5263';

// ============ shared ============

const $ = s => document.querySelector(s);

// ---------- theme ----------
function applyTheme(theme) {
  document.documentElement.dataset.theme = theme;
  localStorage.setItem('theme', theme);
  const btn = $('#theme-toggle');
  if (btn) btn.textContent = theme === 'dark' ? '☀️' : '🌙';
}

function initTheme() {
  const saved = localStorage.getItem('theme');
  const system = matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  applyTheme(saved ?? system);

  $('#theme-toggle')?.addEventListener('click', () => {
    applyTheme(document.documentElement.dataset.theme === 'dark' ? 'light' : 'dark');
  });
}

// prevents HTML injection through user-typed text
function escapeHtml(s) {
  return s.replace(/[&<>"']/g,
    c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
}

// shows API validation errors (400 body) inside the form
async function showError(res) {
  const box = $('#error');
  const problem = await res.json().catch(() => null);
  box.textContent = problem?.errors
    ? Object.values(problem.errors).flat().join('\n')
    : `Request failed (${res.status})`;
  box.classList.remove('hidden');
}

// ---------- page routing ----------
document.addEventListener('DOMContentLoaded', () => {
  initTheme();

  switch (document.body.dataset.page) {
    case 'index':    initIndex();  break;
    case 'create':   initCreate(); break;
    case 'edit':     initEdit();   break;
    case 'details':  initDetails(); break;
  }
});

// ============ index page ============

function initIndex() {
  loadTasks();
}

async function loadTasks() {
  const res = await fetch(`${API}/tasks`);
  if (!res.ok) return alert('Failed to load tasks');
  render(await res.json());
}

function render(tasks) {
  const list = $('#task-list');
  $('#empty').classList.toggle('hidden', tasks.length > 0);
  list.innerHTML = '';

  let dividerShown = false;

  for (const t of tasks) {
    // the famous line: done tasks go below it
    if (t.isCompleted && !dividerShown) {
      dividerShown = true;
      list.insertAdjacentHTML('beforeend',
        '<li class="divider">completed</li>');
    }

    const li = document.createElement('li');
    li.className = 'task' + (t.isCompleted ? ' done' : '');
    li.innerHTML = `
      <span class="dot dot-${t.priority}"></span>
      <div class="task-body">
        <a class="task-title" href="details.html?id=${t.id}">${escapeHtml(t.title)}</a>
        <div class="task-desc">${escapeHtml(t.description)}</div>
      </div>
      <div class="task-actions">
        <button class="btn" data-act="toggle" title="${t.isCompleted ? 'Undo' : 'Done'}">${t.isCompleted ? '↩️' : '✔️'}</button>
        <a class="btn" href="edit.html?id=${t.id}" title="Edit">✏️</a>
        <button class="btn" data-act="delete" title="Delete">🗑️</button>
      </div>`;

    li.querySelector('[data-act=toggle]').onclick = () => toggleTask(t.id);
    li.querySelector('[data-act=delete]').onclick = () => deleteTask(t.id);
    list.appendChild(li);
  }
}

async function toggleTask(id) {
  const res = await fetch(`${API}/tasks/${id}/toggle`, { method: 'PATCH' });
  if (!res.ok) return alert('Task not found');
  loadTasks();   // re-fetch → order updates → item moves across the line
}

async function deleteTask(id) {
  if (!confirm('Delete this task?')) return;
  const res = await fetch(`${API}/tasks/${id}`, { method: 'DELETE' });
  if (!res.ok) return alert('Task not found');
  loadTasks();
}

// ============ create page ============

function initCreate() {
  $('#task-form').addEventListener('submit', async e => {
    e.preventDefault();

    const res = await fetch(`${API}/tasks`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        title: $('#title').value,
        description: $('#description').value,
        priority: Number($('#priority').value)
      })
    });

    if (!res.ok) return showError(res);
    location.href = 'index.html';
  });
}

// ============ edit page ============

async function initEdit() {
  const id = new URLSearchParams(location.search).get('id');
  if (!id) return location.href = 'index.html';

  const res = await fetch(`${API}/tasks/${id}`);
  if (!res.ok) {
    alert('Task not found');
    return location.href = 'index.html';
  }
  const task = await res.json();

  // pre-fill
  $('#title').value = task.title;
  $('#description').value = task.description;
  $('#priority').value = task.priority;

  $('#task-form').addEventListener('submit', async e => {
    e.preventDefault();

    const res = await fetch(`${API}/tasks/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        title: $('#title').value,
        description: $('#description').value,
        priority: Number($('#priority').value),
        isCompleted: task.isCompleted
      })
    });

    if (!res.ok) return showError(res);
    location.href = 'index.html';
  });
}

// ============ details page ============

const PRIORITY_NAMES = ['Low', 'Normal', 'High', 'Critical'];

async function initDetails() {
  const id = new URLSearchParams(location.search).get('id');
  if (!id) return location.href = 'index.html';

  async function load() {
    const res = await fetch(`${API}/tasks/${id}`);
    if (!res.ok) {
      alert('Task not found');
      return location.href = 'index.html';
    }
    const t = await res.json();

    $('#d-dot').className = `dot dot-${t.priority}`;
    $('#d-title').textContent = t.title;
    $('#d-desc').textContent = t.description;
    $('#d-priority').textContent = PRIORITY_NAMES[t.priority];
    $('#d-priority').className = `badge prio-${t.priority}`;
    $('#d-status').textContent = t.isCompleted ? '✅ Done' : '⏳ Pending';
    $('#d-status').className = `badge ${t.isCompleted ? 'badge-done' : 'badge-pending'}`;
    $('#d-toggle').textContent = t.isCompleted ? '↩️ Mark as not done' : '✔️ Mark as done';
    $('#d-edit').href = `edit.html?id=${t.id}`;

    $('#d-toggle').onclick = async () => {
      const r = await fetch(`${API}/tasks/${t.id}/toggle`, { method: 'PATCH' });
      if (!r.ok) return alert('Task not found');
      load();
    };
  }

  load();
}
