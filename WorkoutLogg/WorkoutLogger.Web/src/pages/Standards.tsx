import { useState } from 'react';
import data from '../standards.json';
import { Heading } from '../ui';

export function Standards() {
  const [sex, setSex] = useState('men'); const [weight, setWeight] = useState('83');
  const table = (data as Record<string, Record<string, number[][]>>)[sex];
  return <><Heading eyebrow="ОРИЕНТИРЫ ДЛЯ ПРОГРЕССА" title="Силовые показатели" /><section className="card"><div className="form-grid"><label>Таблица<select value={sex} onChange={e => { setSex(e.target.value); setWeight(e.target.value === 'men' ? '83' : '63'); }}><option value="men">Мужчины</option><option value="women">Женщины</option></select></label><label>Весовая категория, кг<select value={weight} onChange={e => setWeight(e.target.value)}>{Object.keys(table).map(w => <option key={w} value={w}>{w}</option>)}</select></label></div><div className="table-scroll"><table><thead><tr><th>Уровень</th><th>Присед, кг</th><th>Жим лёжа, кг</th><th>Становая, кг</th></tr></thead><tbody>{table[weight]?.map((row, i) => <tr key={i}><th>{['III', 'II', 'I', 'КМС', 'МС'][i]}</th>{row.map((value, index) => <td key={index}>{value}</td>)}</tr>)}</tbody></table></div><p className="muted" style={{ marginTop:20 }}>Справочная таблица клуба. Выбирайте рабочие веса вместе с тренером с учётом вашей подготовки.</p></section></>;
}
