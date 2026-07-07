
// workout-screens.jsx
// All screen components for Workout Logger app design
// Exports to window: WorkoutApp

const VARIANTS = {
  A: {
    name: 'Bold Dark', bg: '#0C0C14', surface: '#14141F', card: '#1C1C2A',
    cardBorder: 'rgba(139,92,246,0.18)', text: '#FFFFFF',
    textSub: 'rgba(255,255,255,0.52)', textMuted: 'rgba(255,255,255,0.28)',
    accent: '#A855F7', accentAlt: '#EC4899',
    accentGrad: 'linear-gradient(135deg, #7C3AED 0%, #A855F7 55%, #EC4899 100%)',
    accentGlow: 'rgba(168,85,247,0.32)', tabBg: '#0C0C14',
    tabBorder: 'rgba(255,255,255,0.07)', dark: true,
    inputBg: '#1C1C2A', pillBg: 'rgba(168,85,247,0.14)', pillBgActive: 'rgba(168,85,247,0.9)',
  },
  B: {
    name: 'Clean Light', bg: '#F2F2FA', surface: '#FFFFFF', card: '#FFFFFF',
    cardBorder: 'rgba(124,58,237,0.1)', text: '#18182E',
    textSub: 'rgba(24,24,46,0.52)', textMuted: 'rgba(24,24,46,0.32)',
    accent: '#7C3AED', accentAlt: '#DB2777',
    accentGrad: 'linear-gradient(135deg, #6D28D9 0%, #7C3AED 55%, #A855F7 100%)',
    accentGlow: 'rgba(124,58,237,0.12)', tabBg: '#FFFFFF',
    tabBorder: 'rgba(0,0,0,0.07)', dark: false,
    inputBg: '#F2F2FA', pillBg: 'rgba(124,58,237,0.09)', pillBgActive: 'rgba(124,58,237,0.88)',
  },
  C: {
    name: 'Glass Dark', bg: '#08081A', surface: 'rgba(255,255,255,0.04)', card: 'rgba(255,255,255,0.07)',
    cardBorder: 'rgba(255,255,255,0.11)', text: '#FFFFFF',
    textSub: 'rgba(255,255,255,0.56)', textMuted: 'rgba(255,255,255,0.3)',
    accent: '#C084FC', accentAlt: '#F472B6',
    accentGrad: 'linear-gradient(135deg, #6D28D9 0%, #C084FC 55%, #F472B6 100%)',
    accentGlow: 'rgba(192,132,252,0.28)', tabBg: 'rgba(10,10,28,0.92)',
    tabBorder: 'rgba(255,255,255,0.1)', dark: true, glass: true,
    inputBg: 'rgba(255,255,255,0.07)', pillBg: 'rgba(192,132,252,0.12)', pillBgActive: 'rgba(192,132,252,0.85)',
  }
};

// ── Micro components ──────────────────────────────────────────

function StatCard({ v, label, value, sub, icon }) {
  return (
    <div style={{
      flex: 1, background: v.card, border: `1px solid ${v.cardBorder}`,
      borderRadius: 16, padding: '14px 12px',
      backdropFilter: v.glass ? 'blur(20px)' : undefined,
    }}>
      <div style={{ fontSize: 18, marginBottom: 4 }}>{icon}</div>
      <div style={{ color: v.text, fontWeight: 700, fontSize: 22, lineHeight: 1 }}>{value}</div>
      {sub && <div style={{ color: v.accent, fontSize: 10, fontWeight: 600, marginTop: 2 }}>{sub}</div>}
      <div style={{ color: v.textMuted, fontSize: 10, marginTop: 4, fontWeight: 500 }}>{label}</div>
    </div>
  );
}

function SectionLabel({ v, children, action }) {
  return (
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
      <span style={{ color: v.text, fontWeight: 700, fontSize: 16 }}>{children}</span>
      {action && <span style={{ color: v.accent, fontSize: 12, fontWeight: 600 }}>{action}</span>}
    </div>
  );
}

function Pill({ v, active, children, onClick }) {
  return (
    <button onClick={onClick} style={{
      background: active ? v.pillBgActive : v.pillBg,
      color: active ? '#fff' : v.textSub,
      border: 'none', borderRadius: 20, padding: '6px 14px',
      fontSize: 12, fontWeight: 600, cursor: 'pointer', whiteSpace: 'nowrap',
      fontFamily: 'inherit',
    }}>{children}</button>
  );
}

// ── Bar Chart (SVG) ──────────────────────────────────────────
function WeekChart({ v }) {
  const days = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
  const vals = [0.6, 0.9, 0.4, 1.0, 0.7, 0.3, 0];
  const today = 3;
  const h = 60, w = 30, gap = 6;
  return (
    <svg width="100%" viewBox={`0 0 ${days.length * (w + gap) - gap} ${h + 22}`} style={{ overflow: 'visible' }}>
      {days.map((d, i) => {
        const barH = vals[i] * h;
        const isToday = i === today;
        const x = i * (w + gap);
        return (
          <g key={d}>
            <rect x={x} y={h - barH} width={w} height={barH} rx={6}
              fill={isToday ? v.accent : (vals[i] > 0 ? `${v.accent}44` : `${v.accent}18`)} />
            {isToday && (
              <rect x={x} y={h - barH} width={w} height={barH} rx={6}
                fill={`url(#barGrad${v.name.replace(' ','')})`} opacity={0.9} />
            )}
            <text x={x + w / 2} y={h + 16} textAnchor="middle" fontSize={9}
              fill={isToday ? v.accent : v.textMuted} fontWeight={isToday ? 700 : 500}
              fontFamily="inherit">{d}</text>
          </g>
        );
      })}
      <defs>
        <linearGradient id={`barGrad${v.name.replace(' ','')}`} x1="0" y1="0" x2="0" y2="1">
          <stop offset="0%" stopColor={v.accentAlt} />
          <stop offset="100%" stopColor={v.accent} />
        </linearGradient>
      </defs>
    </svg>
  );
}

// ── Ring Progress ────────────────────────────────────────────
function Ring({ v, pct, size = 56, label }) {
  const r = (size - 8) / 2;
  const circ = 2 * Math.PI * r;
  return (
    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 4 }}>
      <svg width={size} height={size}>
        <circle cx={size/2} cy={size/2} r={r} fill="none" stroke={`${v.accent}25`} strokeWidth={5} />
        <circle cx={size/2} cy={size/2} r={r} fill="none" stroke={v.accent} strokeWidth={5}
          strokeDasharray={`${circ * pct} ${circ}`}
          strokeLinecap="round" transform={`rotate(-90 ${size/2} ${size/2})`} />
        <text x={size/2} y={size/2 + 5} textAnchor="middle" fontSize={12} fontWeight={700}
          fill={v.text} fontFamily="inherit">{Math.round(pct * 100)}%</text>
      </svg>
      <span style={{ color: v.textMuted, fontSize: 9, fontWeight: 600 }}>{label}</span>
    </div>
  );
}

// ══════════════════════════════════════════════════════════════
// TAB 1 – DASHBOARD
// ══════════════════════════════════════════════════════════════
function DashboardScreen({ v }) {
  return (
    <div style={{ flex: 1, overflowY: 'auto', padding: '0 16px 24px', background: v.bg }}>

      {/* Hero header */}
      <div style={{ marginBottom: 20, paddingTop: 4 }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <div>
            <div style={{ color: v.textMuted, fontSize: 12, fontWeight: 500, marginBottom: 2 }}>
              TUESDAY, 22 APR
            </div>
            <div style={{ color: v.text, fontSize: 22, fontWeight: 800, letterSpacing: -0.5 }}>
              Good morning, Alex 👋
            </div>
          </div>
          <div style={{
            width: 42, height: 42, borderRadius: 14,
            background: v.accentGrad, display: 'flex', alignItems: 'center',
            justifyContent: 'center', fontSize: 18,
            boxShadow: `0 4px 16px ${v.accentGlow}`,
          }}>A</div>
        </div>
      </div>

      {/* Stats row */}
      <div style={{ display: 'flex', gap: 8, marginBottom: 20 }}>
        <StatCard v={v} icon="🏋️" value="124" sub="+3 this week" label="Total Workouts" />
        <StatCard v={v} icon="🔥" value="12" sub="days" label="Streak" />
        <StatCard v={v} icon="⚡" value="4.2t" sub="this week" label="Volume" />
      </div>

      {/* Weekly chart */}
      <div style={{ background: v.card, border: `1px solid ${v.cardBorder}`, borderRadius: 20, padding: '16px 16px 10px', marginBottom: 16, backdropFilter: v.glass ? 'blur(20px)' : undefined }}>
        <SectionLabel v={v} action="Month ›">Weekly Activity</SectionLabel>
        <WeekChart v={v} />
      </div>

      {/* Goals */}
      <div style={{ background: v.card, border: `1px solid ${v.cardBorder}`, borderRadius: 20, padding: 16, marginBottom: 16, backdropFilter: v.glass ? 'blur(20px)' : undefined }}>
        <SectionLabel v={v} action="Edit">Goals</SectionLabel>
        <div style={{ display: 'flex', justifyContent: 'space-around' }}>
          <Ring v={v} pct={0.78} label="Workouts" />
          <Ring v={v} pct={0.55} label="Volume" />
          <Ring v={v} pct={0.9} label="Cardio" />
        </div>
      </div>

      {/* Last workout */}
      <div style={{ background: v.card, border: `1px solid ${v.cardBorder}`, borderRadius: 20, padding: 16, backdropFilter: v.glass ? 'blur(20px)' : undefined }}>
        <SectionLabel v={v} action="See all">Last Workout</SectionLabel>
        <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
          <div style={{ width: 44, height: 44, borderRadius: 12, background: v.accentGrad, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 20, flexShrink: 0, boxShadow: `0 4px 12px ${v.accentGlow}` }}>🏋️</div>
          <div style={{ flex: 1 }}>
            <div style={{ color: v.text, fontWeight: 700, fontSize: 14 }}>Upper Body Push</div>
            <div style={{ color: v.textMuted, fontSize: 11, marginTop: 1 }}>Yesterday · 52 min · 8 exercises</div>
          </div>
          <div style={{ color: v.accent, fontSize: 12, fontWeight: 700 }}>1 280 kg</div>
        </div>
      </div>
    </div>
  );
}

// ══════════════════════════════════════════════════════════════
// TAB 2 – WORKOUTS
// ══════════════════════════════════════════════════════════════
function WorkoutsScreen({ v }) {
  const [filter, setFilter] = React.useState('All');
  const filters = ['All', 'Strength', 'Cardio', 'Stretch'];
  const workouts = [
    { icon: '🏋️', name: 'Upper Body Push', type: 'Strength', date: 'Yesterday', dur: '52 min', vol: '1 280 kg', exs: 8 },
    { icon: '🏃', name: 'Morning Run', type: 'Cardio', date: 'Mon, 21 Apr', dur: '35 min', vol: '6.2 km', exs: 1 },
    { icon: '🏋️', name: 'Leg Day', type: 'Strength', date: 'Sun, 20 Apr', dur: '65 min', vol: '2 440 kg', exs: 7 },
    { icon: '🧘', name: 'Yoga Flow', type: 'Stretch', date: 'Sat, 19 Apr', dur: '40 min', vol: '—', exs: 12 },
    { icon: '🏋️', name: 'Back & Biceps', type: 'Strength', date: 'Fri, 18 Apr', dur: '48 min', vol: '1 060 kg', exs: 6 },
  ];
  const filtered = filter === 'All' ? workouts : workouts.filter(w => w.type === filter);

  return (
    <div style={{ flex: 1, overflowY: 'auto', padding: '0 16px 24px', background: v.bg }}>
      <div style={{ paddingTop: 4, marginBottom: 16 }}>
        <div style={{ color: v.text, fontSize: 24, fontWeight: 800, letterSpacing: -0.5 }}>Workouts</div>
        <div style={{ color: v.textSub, fontSize: 13, marginTop: 2 }}>124 sessions logged</div>
      </div>

      {/* Filters */}
      <div style={{ display: 'flex', gap: 8, marginBottom: 20, overflowX: 'auto', paddingBottom: 4 }}>
        {filters.map(f => <Pill key={f} v={v} active={filter === f} onClick={() => setFilter(f)}>{f}</Pill>)}
      </div>

      {/* Month calendar strip */}
      <div style={{ background: v.card, border: `1px solid ${v.cardBorder}`, borderRadius: 20, padding: '14px 16px', marginBottom: 20, backdropFilter: v.glass ? 'blur(20px)' : undefined }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
          <span style={{ color: v.textSub, fontSize: 12, fontWeight: 600 }}>‹ March</span>
          <span style={{ color: v.text, fontSize: 14, fontWeight: 700 }}>April 2026</span>
          <span style={{ color: v.textSub, fontSize: 12, fontWeight: 600 }}>May ›</span>
        </div>
        <div style={{ display: 'flex', justifyContent: 'space-between' }}>
          {[
            {d:'M',n:20},{d:'T',n:21},{d:'W',n:22,today:true},{d:'T',n:23},{d:'F',n:24},{d:'S',n:25},{d:'S',n:26}
          ].map(item => (
            <div key={item.n} style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 4 }}>
              <span style={{ color: v.textMuted, fontSize: 10, fontWeight: 500 }}>{item.d}</span>
              <div style={{
                width: 30, height: 30, borderRadius: 10, display: 'flex', alignItems: 'center', justifyContent: 'center',
                background: item.today ? v.accentGrad : 'transparent',
                boxShadow: item.today ? `0 4px 12px ${v.accentGlow}` : undefined,
              }}>
                <span style={{ color: item.today ? '#fff' : v.text, fontSize: 13, fontWeight: 700 }}>{item.n}</span>
              </div>
              <div style={{ width: 4, height: 4, borderRadius: 99, background: [20,21,25].includes(item.n) ? v.accent : 'transparent' }} />
            </div>
          ))}
        </div>
      </div>

      {/* Workout list */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
        {filtered.map((w, i) => (
          <div key={i} style={{
            background: v.card, border: `1px solid ${v.cardBorder}`, borderRadius: 18, padding: '14px 16px',
            display: 'flex', alignItems: 'center', gap: 12,
            backdropFilter: v.glass ? 'blur(20px)' : undefined,
          }}>
            <div style={{ width: 44, height: 44, borderRadius: 12, background: v.accentGrad, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 20, flexShrink: 0 }}>{w.icon}</div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ color: v.text, fontWeight: 700, fontSize: 13 }}>{w.name}</div>
              <div style={{ color: v.textMuted, fontSize: 11, marginTop: 1 }}>{w.date} · {w.dur} · {w.exs} exercises</div>
            </div>
            <div style={{ textAlign: 'right', flexShrink: 0 }}>
              <div style={{ color: v.accent, fontWeight: 700, fontSize: 12 }}>{w.vol}</div>
              <div style={{ color: v.textMuted, fontSize: 10 }}>{w.type}</div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

// ══════════════════════════════════════════════════════════════
// TAB 3 – LOGGER
// ══════════════════════════════════════════════════════════════
function LoggerScreen({ v }) {
  const [type, setType] = React.useState('Strength');
  const types = ['Strength', 'Cardio', 'HIIT', 'Stretch'];
  const exercises = [
    { name: 'Bench Press', sets: [{ w: 80, r: 10 }, { w: 85, r: 8 }, { w: 85, r: 7 }] },
    { name: 'Incline DB Press', sets: [{ w: 30, r: 12 }, { w: 32, r: 10 }] },
  ];
  return (
    <div style={{ flex: 1, overflowY: 'auto', padding: '0 16px 24px', background: v.bg }}>
      <div style={{ paddingTop: 4, marginBottom: 16, display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div>
          <div style={{ color: v.text, fontSize: 24, fontWeight: 800, letterSpacing: -0.5 }}>Logger</div>
          <div style={{ color: v.textSub, fontSize: 13, marginTop: 2 }}>Today · 22 Apr 2026</div>
        </div>
        <div style={{
          background: v.accentGrad, color: '#fff', border: 'none',
          borderRadius: 12, padding: '8px 14px', fontSize: 12, fontWeight: 700,
          cursor: 'pointer', boxShadow: `0 4px 14px ${v.accentGlow}`,
        }}>+ New</div>
      </div>

      {/* Date strip */}
      <div style={{ display: 'flex', gap: 6, marginBottom: 20, overflowX: 'auto', paddingBottom: 2 }}>
        {[{d:'Mon',n:20},{d:'Tue',n:21},{d:'Wed',n:22,active:true},{d:'Thu',n:23},{d:'Fri',n:24}].map(item => (
          <div key={item.n} style={{
            display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 4,
            background: item.active ? v.accentGrad : v.card,
            border: `1px solid ${item.active ? 'transparent' : v.cardBorder}`,
            borderRadius: 14, padding: '8px 14px', flexShrink: 0,
            boxShadow: item.active ? `0 4px 16px ${v.accentGlow}` : undefined,
            backdropFilter: !item.active && v.glass ? 'blur(20px)' : undefined,
          }}>
            <span style={{ color: item.active ? 'rgba(255,255,255,0.7)' : v.textMuted, fontSize: 10, fontWeight: 600 }}>{item.d}</span>
            <span style={{ color: item.active ? '#fff' : v.text, fontSize: 16, fontWeight: 800 }}>{item.n}</span>
          </div>
        ))}
      </div>

      {/* Type selector */}
      <div style={{ display: 'flex', gap: 8, marginBottom: 20, overflowX: 'auto', paddingBottom: 2 }}>
        {types.map(t => <Pill key={t} v={v} active={type === t} onClick={() => setType(t)}>{t}</Pill>)}
      </div>

      {/* Exercises */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: 12, marginBottom: 16 }}>
        {exercises.map((ex, i) => (
          <div key={i} style={{ background: v.card, border: `1px solid ${v.cardBorder}`, borderRadius: 18, padding: 16, backdropFilter: v.glass ? 'blur(20px)' : undefined }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
              <span style={{ color: v.text, fontWeight: 700, fontSize: 14 }}>{ex.name}</span>
              <span style={{ color: v.accent, fontSize: 12, fontWeight: 600 }}>+ Set</span>
            </div>
            <div style={{ display: 'flex', gap: 6, marginBottom: 8 }}>
              <span style={{ color: v.textMuted, fontSize: 10, fontWeight: 600, width: 24, textAlign: 'center' }}>SET</span>
              <span style={{ color: v.textMuted, fontSize: 10, fontWeight: 600, flex: 1, textAlign: 'center' }}>KG</span>
              <span style={{ color: v.textMuted, fontSize: 10, fontWeight: 600, flex: 1, textAlign: 'center' }}>REPS</span>
              <span style={{ width: 20 }} />
            </div>
            {ex.sets.map((s, j) => (
              <div key={j} style={{ display: 'flex', gap: 6, alignItems: 'center', marginBottom: 6 }}>
                <div style={{ width: 24, height: 24, borderRadius: 8, background: v.pillBgActive, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                  <span style={{ color: '#fff', fontSize: 10, fontWeight: 700 }}>{j + 1}</span>
                </div>
                <div style={{ flex: 1, background: v.inputBg, borderRadius: 10, padding: '6px 10px', textAlign: 'center', border: `1px solid ${v.cardBorder}` }}>
                  <span style={{ color: v.text, fontSize: 13, fontWeight: 700 }}>{s.w}</span>
                </div>
                <div style={{ flex: 1, background: v.inputBg, borderRadius: 10, padding: '6px 10px', textAlign: 'center', border: `1px solid ${v.cardBorder}` }}>
                  <span style={{ color: v.text, fontSize: 13, fontWeight: 700 }}>{s.r}</span>
                </div>
                <div style={{ width: 20, height: 20, display: 'flex', alignItems: 'center', justifyContent: 'center', color: v.textMuted, fontSize: 16 }}>×</div>
              </div>
            ))}
          </div>
        ))}
      </div>

      {/* Add exercise */}
      <button style={{
        width: '100%', background: 'transparent', border: `2px dashed ${v.accent}55`,
        borderRadius: 18, padding: '14px', color: v.accent, fontWeight: 700, fontSize: 14,
        cursor: 'pointer', fontFamily: 'inherit',
      }}>+ Add Exercise</button>

      {/* Timer */}
      <div style={{
        marginTop: 20, background: v.accentGrad, borderRadius: 18, padding: '14px 20px',
        display: 'flex', justifyContent: 'space-between', alignItems: 'center',
        boxShadow: `0 8px 24px ${v.accentGlow}`,
      }}>
        <span style={{ color: '#fff', fontWeight: 700, fontSize: 14 }}>⏱ Rest Timer</span>
        <span style={{ color: 'rgba(255,255,255,0.9)', fontWeight: 800, fontSize: 20, letterSpacing: 1 }}>01:30</span>
        <span style={{ color: 'rgba(255,255,255,0.8)', fontSize: 12 }}>▶ Start</span>
      </div>
    </div>
  );
}

// ══════════════════════════════════════════════════════════════
// TAB 4 – PROFILE
// ══════════════════════════════════════════════════════════════
function ProfileScreen({ v }) {
  const prs = [
    { name: 'Bench Press', val: '105 kg' },
    { name: 'Squat', val: '140 kg' },
    { name: 'Deadlift', val: '180 kg' },
    { name: '5K Run', val: '22:14' },
  ];
  const badges = ['🔥', '💪', '🏆', '⚡', '🎯', '🌟'];
  const settings = ['Notifications', 'Units (kg / lbs)', 'Language', 'Privacy', 'Premium ✦'];
  return (
    <div style={{ flex: 1, overflowY: 'auto', background: v.bg }}>
      {/* Hero */}
      <div style={{ background: v.accentGrad, padding: '24px 20px 32px', position: 'relative', boxShadow: `0 12px 32px ${v.accentGlow}` }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
          <div style={{ width: 64, height: 64, borderRadius: 20, background: 'rgba(255,255,255,0.25)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 28, border: '2px solid rgba(255,255,255,0.4)', backdropFilter: 'blur(10px)' }}>A</div>
          <div>
            <div style={{ color: '#fff', fontSize: 20, fontWeight: 800 }}>Alex Petrov</div>
            <div style={{ color: 'rgba(255,255,255,0.72)', fontSize: 12, marginTop: 2 }}>Member since Jan 2024 · ✦ Premium</div>
          </div>
        </div>
        <div style={{ display: 'flex', gap: 0, marginTop: 20, background: 'rgba(255,255,255,0.15)', borderRadius: 16, padding: '12px 0', backdropFilter: 'blur(10px)' }}>
          {[['82 kg', 'Weight'], ['183 cm', 'Height'], ['22%', 'Body Fat']].map(([val, lbl], i) => (
            <div key={lbl} style={{ flex: 1, textAlign: 'center', borderRight: i < 2 ? '1px solid rgba(255,255,255,0.2)' : undefined }}>
              <div style={{ color: '#fff', fontWeight: 800, fontSize: 18 }}>{val}</div>
              <div style={{ color: 'rgba(255,255,255,0.65)', fontSize: 10, fontWeight: 500, marginTop: 2 }}>{lbl}</div>
            </div>
          ))}
        </div>
      </div>

      <div style={{ padding: '20px 16px 24px', display: 'flex', flexDirection: 'column', gap: 16 }}>
        {/* PRs */}
        <div style={{ background: v.card, border: `1px solid ${v.cardBorder}`, borderRadius: 20, padding: 16, backdropFilter: v.glass ? 'blur(20px)' : undefined }}>
          <SectionLabel v={v} action="History ›">Personal Records</SectionLabel>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 8 }}>
            {prs.map(pr => (
              <div key={pr.name} style={{ background: v.pillBg, borderRadius: 14, padding: '10px 12px' }}>
                <div style={{ color: v.textMuted, fontSize: 10, fontWeight: 600, marginBottom: 2 }}>{pr.name}</div>
                <div style={{ color: v.accent, fontSize: 18, fontWeight: 800 }}>{pr.val}</div>
              </div>
            ))}
          </div>
        </div>

        {/* Achievements */}
        <div style={{ background: v.card, border: `1px solid ${v.cardBorder}`, borderRadius: 20, padding: 16, backdropFilter: v.glass ? 'blur(20px)' : undefined }}>
          <SectionLabel v={v} action="All 18 ›">Achievements</SectionLabel>
          <div style={{ display: 'flex', gap: 10 }}>
            {badges.map((b, i) => (
              <div key={i} style={{ width: 40, height: 40, borderRadius: 12, background: i < 4 ? v.pillBgActive : v.pillBg, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 20, opacity: i < 4 ? 1 : 0.4 }}>{b}</div>
            ))}
          </div>
        </div>

        {/* Settings */}
        <div style={{ background: v.card, border: `1px solid ${v.cardBorder}`, borderRadius: 20, overflow: 'hidden', backdropFilter: v.glass ? 'blur(20px)' : undefined }}>
          {settings.map((s, i) => (
            <div key={s} style={{
              padding: '14px 16px', display: 'flex', justifyContent: 'space-between', alignItems: 'center',
              borderBottom: i < settings.length - 1 ? `1px solid ${v.cardBorder}` : undefined,
            }}>
              <span style={{ color: s.includes('Premium') ? v.accent : v.text, fontWeight: s.includes('Premium') ? 700 : 500, fontSize: 14 }}>{s}</span>
              <span style={{ color: v.textMuted, fontSize: 16 }}>›</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

// ══════════════════════════════════════════════════════════════
// BOTTOM TAB BAR
// ══════════════════════════════════════════════════════════════
const TAB_ICONS = {
  Dashboard: ['M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6', 'dashboard'],
  WorkOuts: ['M4 6h16M4 10h16M4 14h16M4 18h16', 'list'],
  Logger: ['M12 4v16m8-8H4', 'plus'],
  Profile: ['M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z', 'person'],
};

function BottomTabBar({ v, activeTab, onTabChange }) {
  const tabs = ['Dashboard', 'WorkOuts', 'Logger', 'Profile'];
  return (
    <div style={{
      display: 'flex', background: v.tabBg, borderTop: `1px solid ${v.tabBorder}`,
      padding: '8px 4px 4px', backdropFilter: v.glass ? 'blur(40px)' : undefined,
    }}>
      {tabs.map(tab => {
        const active = tab === activeTab;
        const isLogger = tab === 'Logger';
        return (
          <button key={tab} onClick={() => onTabChange(tab)} style={{
            flex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 3,
            background: 'transparent', border: 'none', cursor: 'pointer', padding: '4px 2px',
            position: 'relative',
          }}>
            {isLogger ? (
              <div style={{
                width: 44, height: 44, borderRadius: 14, background: v.accentGrad,
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                boxShadow: `0 4px 16px ${v.accentGlow}`, marginBottom: 2,
              }}>
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#fff" strokeWidth="2.5" strokeLinecap="round">
                  <path d="M12 4v16m8-8H4" />
                </svg>
              </div>
            ) : (
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none"
                stroke={active ? v.accent : v.textMuted} strokeWidth={active ? 2.2 : 1.8} strokeLinecap="round" strokeLinejoin="round">
                <path d={TAB_ICONS[tab][0]} />
              </svg>
            )}
            {!isLogger && (
              <span style={{ color: active ? v.accent : v.textMuted, fontSize: 10, fontWeight: active ? 700 : 500, fontFamily: 'inherit' }}>{tab}</span>
            )}
            {active && !isLogger && (
              <div style={{ width: 4, height: 4, borderRadius: 99, background: v.accent, position: 'absolute', bottom: 0 }} />
            )}
          </button>
        );
      })}
    </div>
  );
}

// ══════════════════════════════════════════════════════════════
// MAIN WORKOUT APP (phone prototype)
// ══════════════════════════════════════════════════════════════
function WorkoutApp({ variantKey = 'A', initialTab = 'Dashboard' }) {
  const v = VARIANTS[variantKey];
  const [tab, setTab] = React.useState(initialTab);
  const screens = { Dashboard: DashboardScreen, WorkOuts: WorkoutsScreen, Logger: LoggerScreen, Profile: ProfileScreen };
  const Screen = screens[tab];
  return (
    <div style={{
      width: '100%', height: '100%', display: 'flex', flexDirection: 'column',
      background: v.bg, fontFamily: "'Plus Jakarta Sans', sans-serif", overflow: 'hidden',
    }}>
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden', minHeight: 0 }}>
        <Screen v={v} />
      </div>
      <BottomTabBar v={v} activeTab={tab} onTabChange={setTab} />
    </div>
  );
}

// Export all to window
Object.assign(window, { WorkoutApp, VARIANTS, DashboardScreen, WorkoutsScreen, LoggerScreen, ProfileScreen, BottomTabBar });
