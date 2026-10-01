import { useEffect, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { CalendarDays, Gift, MessageSquare, Send, Wallet } from 'lucide-react';
import { useApp } from '../App';
import { api } from '../api';
import { useAction, useApi } from '../hooks';
import { date, Empty, Heading, Loading, Notice } from '../ui';
import type { Slot } from './Trainers';

const statuses: Record<string, string> = { Pending: 'Ожидает подтверждения', Confirmed: 'Подтверждено', Cancelled: 'Отменено', Completed: 'Завершено', NoShow: 'Не состоялось', Accepted: 'Принята', Declined: 'Отклонена', Expired: 'Истекла' };
type Booking = { id: string; status: string; studentNote?: string; slot?: Slot };
type TrainingRequest = { id: string; status: string; message?: string; createdAtUtc: string };

export function Bookings() {
  const bookings = useApi<Booking[]>('/api/schedule/bookings/my');
  const requests = useApi<TrainingRequest[]>('/api/trainers/requests/my');
  const action = useAction(); const [cancelId, setCancelId] = useState<string>(); const [reason, setReason] = useState('');
  return <><Heading eyebrow="ВРЕМЯ ДЛЯ СЕБЯ" title="Мои записи"><Link className="button primary" to="/trainers">Найти тренера</Link></Heading><Notice error={bookings.error || requests.error} />{bookings.loading ? <Loading /> : <div className="card list-card">{!bookings.data?.length ? <Empty><CalendarDays size={32} /><h3>Расписание пока свободно</h3><p>Выбери тренера и удобное время для занятия.</p></Empty> : bookings.data.map(b => <div className="list-row" key={b.id}><span className="row-icon"><CalendarDays size={20} /></span><span><strong>{b.slot ? `${date(b.slot.startUtc)} · ${new Date(b.slot.startUtc).toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })}` : 'Занятие с тренером'}</strong><small>{statuses[b.status] ?? b.status} · {b.studentNote}</small></span>{['Pending', 'Confirmed'].includes(b.status) && <button className="secondary" onClick={() => setCancelId(b.id)}>Отменить запись</button>}</div>)}</div>}
    <div className="section-heading"><h2>Мои заявки тренерам</h2></div><div className="card">{requests.loading ? <Loading /> : !requests.data?.length ? <p className="muted">Заявок пока нет.</p> : requests.data.map(r => <div className="list-row" key={r.id}><span><strong>{r.message || 'Заявка на тренировку'}</strong><small>{date(r.createdAtUtc)} · {statuses[r.status] ?? r.status}</small></span>{r.status === 'Pending' && <button className="secondary" disabled={action.busy} onClick={() => action.run(async () => { await api(`/api/trainers/requests/${r.id}/cancel`, 'POST'); requests.reload(); }, 'Заявка отменена')}>Отозвать</button>}</div>)}<Notice {...action} /></div>
    {cancelId && <div className="dialog-backdrop"><section className="card dialog" role="dialog" aria-modal="true" aria-labelledby="cancel-title"><h2 id="cancel-title">Отменить запись?</h2><p>При поздней отмене могут действовать условия тренера.</p><label>Причина<textarea maxLength={1000} value={reason} onChange={e => setReason(e.target.value)} /></label><div className="form-actions"><button className="secondary" onClick={() => setCancelId(undefined)}>Оставить запись</button><button className="danger" disabled={action.busy} onClick={() => action.run(async () => { await api(`/api/schedule/bookings/${cancelId}/cancel`, 'POST', { reason }); setCancelId(undefined); bookings.reload(); }, 'Запись отменена')}>Отменить</button></div><Notice {...action} /></section></div>}
  </>;
}

type Conversation = { id: string; otherName: string; lastMessageText?: string; unreadCount: number; trainerUserId: string; studentUserId: string };
type Message = { id: string; senderUserId: string; text: string; sentAtUtc: string };

export function Chats() {
  const conversations = useApi<Conversation[]>('/api/web/conversations'); const [params, setParams] = useSearchParams();
  const selected = params.get('conversation');
  return <><Heading eyebrow="НА СВЯЗИ С КЛУБОМ" title="Сообщения"><button className="secondary" onClick={conversations.reload}>Обновить список</button></Heading><Notice error={conversations.error} />{conversations.loading ? <Loading /> : !conversations.data?.length ? <Empty><MessageSquare size={32} /><h3>Диалог начинается с заявки</h3><p>Открой карточку тренера, отправь заявку и напиши ему.</p><Link to="/trainers" className="button secondary">К тренерам</Link></Empty> : <div className="chat-layout"><aside className="card list-card">{conversations.data.map((c, i) => <button className={`conversation ${c.id === selected ? 'selected' : ''}`} key={c.id} onClick={() => setParams({ conversation: c.id })}><span className="avatar"><MessageSquare size={20} /></span><span><strong>{c.otherName}</strong><small>{c.lastMessageText || 'Напиши первое сообщение'}</small></span>{c.unreadCount > 0 && <span className="badge">{c.unreadCount}</span>}</button>)}</aside>{selected ? <ChatThread key={selected} id={selected} /> : <Empty>Выбери диалог слева</Empty>}</div>}</>;
}

function ChatThread({ id }: { id: string }) {
  const { user } = useApp(); const [messages, setMessages] = useState<Message[]>([]); const [text, setText] = useState(''); const [error, setError] = useState(''); const [page, setPage] = useState(1); const [total, setTotal] = useState(0);
  const action = useAction();
  useEffect(() => {
    let active = true;
    const load = async () => {
      try {
        const result = await api<{ items: Message[]; totalCount: number }>(`/api/chat/conversations/${id}/messages?page=${page}&pageSize=50`);
        if (active) { setMessages(result.items); setTotal(result.totalCount); setError(''); }
        await api(`/api/chat/conversations/${id}/read`, 'POST');
      } catch (e) { if (active) setError((e as Error).message); }
    };
    void load(); const timer = window.setInterval(load, 10000);
    return () => { active = false; clearInterval(timer); };
  }, [id, page]);
  return <section className="card chat-thread"><div className="section-heading"><h2>Диалог с клубом</h2><span className="muted">Обновляется каждые 10 с</span></div><Notice error={error} /><div className="messages" aria-live="polite">{!messages.length ? <Empty>Сообщений пока нет</Empty> : messages.map(m => <article key={m.id} className={`message ${m.senderUserId === user.id ? 'mine' : ''}`}><p>{m.text}</p><small>{new Date(m.sentAtUtc).toLocaleString('ru-RU', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' })}</small></article>)}</div><div className="pagination"><button className="link-button" disabled={page * 50 >= total} onClick={() => setPage(p => p + 1)}>Старые сообщения</button><button className="link-button" disabled={page === 1} onClick={() => setPage(p => p - 1)}>Новые сообщения</button></div><form className="message-form" onSubmit={e => { e.preventDefault(); void action.run(async () => { const message = await api<Message>(`/api/chat/conversations/${id}/messages`, 'POST', { text }); setText(''); setPage(1); setMessages(items => [...items, message]); }); }}><input aria-label="Текст сообщения" placeholder="Напиши сообщение…" required maxLength={2000} value={text} onChange={e => setText(e.target.value)} /><button className="primary" aria-label="Отправить сообщение" disabled={action.busy || !text.trim()}><Send size={19} /></button></form><Notice {...action} /></section>;
}

export function WalletPage() {
  const wallet = useApi<{ balance: number }>('/api/wallet'); const [page, setPage] = useState(1);
  const history = useApi<{ items: { id: string; amount: number; description: string; type: string; createdAtUtc: string }[]; totalCount: number }>(`/api/wallet/history?page=${page}&pageSize=20`);
  const action = useAction();
  return <><Heading eyebrow="ТВОЯ АКТИВНОСТЬ ЦЕНИТСЯ" title="Кошелёк" /><Notice error={wallet.error || history.error} /><div className="stats-grid"><section className="card balance"><Wallet size={26} /><p>Доступный баланс</p><strong>{wallet.data?.balance ?? '—'} <small>FC</small></strong><span className="muted">FitCoins для занятий с тренерами</span></section><section className="card"><Gift size={26} /><h2>Семь дней в движении</h2><p className="muted">Бонус за серию тренировок. Проверим твою историю занятий.</p><button className="primary" disabled={action.busy} onClick={() => action.run(async () => { await api('/api/wallet/rewards/streak', 'POST'); wallet.reload(); history.reload(); }, 'Бонус получен!')}>Получить бонус</button><Notice {...action} /></section></div><div className="section-heading"><h2>История операций</h2></div><div className="card">{history.loading ? <Loading /> : !history.data?.items.length ? <Empty>Операций пока нет</Empty> : history.data.items.map(item => <div className="list-row" key={item.id}><span><strong>{item.description || item.type}</strong><small>{date(item.createdAtUtc)}</small></span><strong>{item.amount > 0 ? '+' : ''}{item.amount} FC</strong></div>)}</div><div className="pagination"><button className="secondary" disabled={page === 1} onClick={() => setPage(p => p - 1)}>Назад</button><span>Страница {page}</span><button className="secondary" disabled={page * 20 >= (history.data?.totalCount ?? 0)} onClick={() => setPage(p => p + 1)}>Далее</button></div></>;
}
