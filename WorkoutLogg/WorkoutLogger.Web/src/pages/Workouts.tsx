import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, ArrowUpRight, Dumbbell, Plus, Trash2 } from 'lucide-react';
import { api } from '../api';
import { useAction, useApi } from '../hooks';
import { date, Empty, Heading, Loading, Notice } from '../ui';
import { workoutNames, type Exercise, type Workout, type WorkoutDetail } from '../types';

export function Workouts() {
  const [page, setPage] = useState(1);
  const { data, loading, error } = useApi<Workout[]>(`/Workouts?page=${page}&pageSize=20`);
  return <><Heading eyebrow="ДНЕВНИК ДВИЖЕНИЯ" title="Мои тренировки"><Link className="button primary" to="/workouts/new"><Plus size={18} />Новая тренировка</Link></Heading><Notice error={error} />{loading ? <Loading /> : <div className="card list-card">{!data?.length ? <Empty><Dumbbell size={34} /><h3>Здесь будет твой прогресс</h3><p>Записывай упражнения, вес и повторения после каждого занятия.</p></Empty> : data.map(w => <Link className="list-row" to={`/workouts/${w.id}`} key={w.id}><span className="row-icon"><Dumbbell size={22} /></span><span><strong>{workoutNames[w.workoutType] ?? w.workoutType}</strong><small>{date(w.startDate)} · Упражнений: {w.exerciseCount}</small></span><span className="muted">{Math.max(0, Math.round((+new Date(w.endDate) - +new Date(w.startDate)) / 60000))} мин</span><ArrowUpRight size={20} /></Link>)}</div>}<div className="pagination"><button className="secondary" disabled={page === 1 || loading} onClick={() => setPage(p => p - 1)}>Назад</button><span>Страница {page}</span><button className="secondary" disabled={!data || data.length < 20 || loading} onClick={() => setPage(p => p + 1)}>Далее</button></div></>;
}

const localDate = (date: Date) => new Date(date.getTime() - date.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
const newExercise = (): Exercise => ({ name: '', description: '', complexity: 'Middle', sets: [{ setNumber: 1, reps: 10, weightKg: 0, restSeconds: 60, isWarmup: false }] });

export function WorkoutEditor() {
  const { id } = useParams(); const navigate = useNavigate();
  const load = useApi<WorkoutDetail>(id ? `/api/web/workouts/${id}` : null);
  const [type, setType] = useState('Strength'); const [start, setStart] = useState(localDate(new Date())); const [end, setEnd] = useState(localDate(new Date(Date.now() + 3600000)));
  const [exercises, setExercises] = useState<Exercise[]>([newExercise()]); const [deleting, setDeleting] = useState(false);
  const action = useAction();
  useEffect(() => { if (load.data) { setType(load.data.workoutType); setStart(localDate(new Date(load.data.startDate))); setEnd(localDate(new Date(load.data.endDate))); setExercises(load.data.exercises); } }, [load.data]);
  const update = (index: number, value: Partial<Exercise>) => setExercises(items => items.map((e, i) => i === index ? { ...e, ...value } : e));
  if (id && load.loading) return <Loading />;
  if (load.error) return <Notice error={load.error} />;
  return <><Link className="text-link" to="/workouts"><ArrowLeft size={17} />К тренировкам</Link><Heading eyebrow="КАЖДЫЙ ПОДХОД ИМЕЕТ ЗНАЧЕНИЕ" title={id ? 'Твоя тренировка' : 'Новая тренировка'} />
    <form onSubmit={e => { e.preventDefault(); void action.run(async () => {
      const body = { workoutType: type, startDate: new Date(start).toISOString(), endDate: new Date(end).toISOString(), exercises };
      await api(id ? `/Workouts/${id}` : '/Workouts', id ? 'PUT' : 'POST', body); navigate('/workouts');
    }); }}>
      <section className="card form-grid three"><label>Вид тренировки<select value={type} onChange={e => setType(e.target.value)}>{Object.entries(workoutNames).map(([key, name]) => <option key={key} value={key}>{name}</option>)}</select></label><label>Начало<input type="datetime-local" required value={start} onChange={e => setStart(e.target.value)} /></label><label>Окончание<input type="datetime-local" min={start} required value={end} onChange={e => setEnd(e.target.value)} /></label></section>
      {exercises.map((exercise, index) => <section className="card exercise" key={index}><div className="section-heading"><span className="eyebrow">УПРАЖНЕНИЕ {String(index + 1).padStart(2, '0')}</span><button type="button" className="icon-button" aria-label={`Удалить упражнение ${index + 1}`} onClick={() => setExercises(items => items.filter((_, i) => i !== index))}><Trash2 size={17} /></button></div><div className="form-grid"><label>Название<input required maxLength={200} placeholder="Например, жим лёжа" value={exercise.name} onChange={e => update(index, { name: e.target.value })} /></label><label>Сложность<select value={exercise.complexity} onChange={e => update(index, { complexity: e.target.value })}><option value="Low">Лёгкая</option><option value="Middle">Средняя</option><option value="High">Высокая</option></select></label></div>
        <div className="sets"><div className="set-row set-labels"><span>Подход</span><span>Вес, кг</span><span>Повторы</span><span>Отдых, с</span><span>Разминка</span><span /></div>{exercise.sets.map((set, si) => <div className="set-row" key={si}><strong>{si + 1}</strong>{(['weightKg', 'reps', 'restSeconds'] as const).map(key => <input aria-label={`${key === 'weightKg' ? 'Вес' : key === 'reps' ? 'Повторения' : 'Отдых'}, подход ${si + 1}`} key={key} type="number" required min={0} step={key === 'weightKg' ? .5 : 1} max={key === 'weightKg' ? 2000 : key === 'reps' ? 10000 : 86400} value={set[key]} onChange={e => update(index, { sets: exercise.sets.map((s, i) => i === si ? { ...s, [key]: +e.target.value } : s) })} />)}<input type="checkbox" aria-label={`Разминка, подход ${si + 1}`} checked={set.isWarmup} onChange={e => update(index, { sets: exercise.sets.map((s, i) => i === si ? { ...s, isWarmup: e.target.checked } : s) })} /><button type="button" className="icon-button" aria-label={`Удалить подход ${si + 1}`} onClick={() => update(index, { sets: exercise.sets.filter((_, i) => i !== si).map((s, i) => ({ ...s, setNumber: i + 1 })) })}><Trash2 size={15} /></button></div>)}</div>
        <button type="button" className="secondary" onClick={() => update(index, { sets: [...exercise.sets, { ...(exercise.sets.at(-1) ?? newExercise().sets[0]), setNumber: exercise.sets.length + 1 }] })}><Plus size={16} />Добавить подход</button>
      </section>)}
      <button className="secondary" type="button" onClick={() => setExercises(items => [...items, newExercise()])}><Plus size={18} />Добавить упражнение</button><Notice {...action} /><div className="form-actions"><button className="primary" disabled={action.busy || !exercises.length}>Сохранить тренировку</button>{id && <button className="danger" type="button" onClick={() => setDeleting(true)}>Удалить тренировку</button>}</div>
    </form>{deleting && <div className="dialog-backdrop"><section className="card dialog" role="alertdialog" aria-modal="true" aria-labelledby="delete-title"><h2 id="delete-title">Удалить тренировку?</h2><p>Запись и все её подходы будут удалены.</p><div className="form-actions"><button className="secondary" onClick={() => setDeleting(false)}>Оставить</button><button className="danger" disabled={action.busy} onClick={() => action.run(async () => { await api(`/Workouts/${id}`, 'DELETE'); navigate('/workouts'); })}>Удалить</button></div><Notice {...action} /></section></div>}
  </>;
}
