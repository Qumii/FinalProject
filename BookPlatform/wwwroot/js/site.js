function showToast(msg) {
    var t = document.getElementById('toast');
    if (!t) return;
    t.textContent = msg;
    t.classList.add('show');
    setTimeout(function () { t.classList.remove('show'); }, 2200);
}

function getToken(form) {
    var el = form.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
}

/* ---------- star rating input ---------- */
document.addEventListener('click', function (e) {
    var btn = e.target.closest('.star-input button');
    if (!btn) return;
    var container = btn.closest('.star-input');
    var value = parseInt(btn.getAttribute('data-value'), 10);
    var hidden = container.parentElement.querySelector('input[name="Rating"]');
    if (hidden) hidden.value = value;
    var buttons = container.querySelectorAll('button');
    buttons.forEach(function (b) {
        var v = parseInt(b.getAttribute('data-value'), 10);
        b.classList.toggle('on', v <= value);
    });
});

/* ---------- shelf add/remove (AJAX) ---------- */
document.addEventListener('click', function (e) {
    var btn = e.target.closest('.shelf-btn');
    if (!btn) return;
    e.preventDefault();

    var bookId = btn.getAttribute('data-book-id');
    var onShelf = btn.getAttribute('data-on-shelf') === 'true';
    var url = onShelf ? '/Shelf/Remove' : '/Shelf/Add';
    var form = btn.closest('form') || document.getElementById('globalAntiForgeryForm');
    var token = form ? getToken(form) : '';

    var body = new URLSearchParams();
    body.append('bookId', bookId);
    if (token) body.append('__RequestVerificationToken', token);

    fetch(url, { method: 'POST', body: body })
        .then(function (r) {
            if (r.status === 401 || r.redirected) { window.location.href = '/Account/Login'; return null; }
            return r.json();
        })
        .then(function (data) {
            if (!data) return;
            btn.setAttribute('data-on-shelf', data.onShelf ? 'true' : 'false');
            btn.textContent = data.onShelf ? '✓ Rəfimdə' : '+ Rəfə əlavə et';
            btn.classList.toggle('shelf-on', data.onShelf);
            showToast(data.onShelf ? 'Rəfə əlavə olundu' : 'Rəfdən silindi');
            if (btn.hasAttribute('data-remove-on-toggle') && !data.onShelf) {
                var card = btn.closest('.shelf-card-wrap');
                if (card) card.remove();
            }
        })
        .catch(function () { showToast('Xəta baş verdi'); });
});

/* ---------- review submit (AJAX) ---------- */
document.addEventListener('submit', function (e) {
    var form = e.target;
    if (!form.matches('#reviewForm')) return;
    e.preventDefault();

    var bookId = form.getAttribute('data-book-id');
    var rating = form.querySelector('input[name="Rating"]').value;
    var text = form.querySelector('textarea[name="Text"]').value.trim();
    var token = getToken(form);

    if (!rating || rating === '0' || !text) {
        showToast('Ulduz seç və rəy mətnini yaz');
        return;
    }

    var body = new URLSearchParams();
    body.append('BookId', bookId);
    body.append('Rating', rating);
    body.append('Text', text);
    body.append('__RequestVerificationToken', token);

    fetch('/Books/AddReview', { method: 'POST', body: body })
        .then(function (r) {
            if (r.status === 401) { window.location.href = '/Account/Login'; return null; }
            return r.json();
        })
        .then(function (data) {
            if (!data) return;
            if (!data.success) { showToast(data.message || 'Xəta baş verdi'); return; }

            document.getElementById('avgNum').textContent = data.averageRating.toFixed(1);
            document.getElementById('rcountNum').textContent = data.reviewCount + ' rəy';
            document.getElementById('avgStars').textContent = starString(data.averageRating);

            var list = document.getElementById('reviewList');
            list.innerHTML = '';
            data.reviews.forEach(function (rv) {
                var div = document.createElement('div');
                div.className = 'review';
                div.innerHTML = '<div class="rhead"><span class="rname"></span><span class="rstars"></span></div><p></p>';
                div.querySelector('.rname').textContent = rv.userName;
                div.querySelector('.rstars').textContent = starString(rv.rating);
                div.querySelector('p').textContent = rv.text;
                list.appendChild(div);
            });

            form.querySelector('textarea[name="Text"]').value = '';
            form.querySelector('input[name="Rating"]').value = '0';
            form.querySelectorAll('.star-input button').forEach(function (b) { b.classList.remove('on'); });

            showToast('Rəyin əlavə olundu!');
        })
        .catch(function () { showToast('Xəta baş verdi'); });
});

function starString(n) {
    var full = Math.round(n);
    return '★★★★★'.slice(0, full) + '☆☆☆☆☆'.slice(0, 5 - full);
}
