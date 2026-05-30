export const daysOfWeek = [
  { value: 'mon', label: 'Lunes' },
  { value: 'tue', label: 'Martes' },
  { value: 'wed', label: 'Miercoles' },
  { value: 'thu', label: 'Jueves' },
  { value: 'fri', label: 'Viernes' },
  { value: 'sat', label: 'Sabado' },
  { value: 'sun', label: 'Domingo' },
];

export const dayBits = {
  mon: 1,
  tue: 2,
  wed: 4,
  thu: 8,
  fri: 16,
  sat: 32,
  sun: 64,
};

export const airportCodePattern = /^[A-Za-z0-9]{3}$/;

export const BUSINESS_ID_COOKIE = 'businessId';

export const AIRLINE_ID = 1;

export const modalFieldLabels = {
  origin: 'Aeropuerto origen',
  destination: 'Aeropuerto destino',
  departureTime: 'Hora de salida',
  arrivalTime: 'Hora de llegada',
  duration: 'Duracion',
  frequency: 'Frecuencia',
  aircraftId: 'Aeronave',
  touristPrice: 'Precio Turista',
  firstClassPrice: 'Precio Primera Clase',
  carryOnPrice: 'Precio Equipaje de Mano',
  checkedPrice: 'Precio Equipaje Documentado',
};

export function normalizeCode(value) {
  return String(value || '').toUpperCase().trim();
}

export function normalizeTerm(value) {
  return String(value || '').trim();
}

export function getSuggestionCode(suggestion) {
  return suggestion?.airportCode || suggestion?.AirportCode || '';
}

export function getSuggestionLabel(suggestion) {
  const displayName = suggestion?.displayName || suggestion?.DisplayName || '';
  if (displayName) return displayName;
  const name = suggestion?.name || suggestion?.Name || '';
  const country = suggestion?.country || suggestion?.Country || '';

  if (name && country) return `${name}, ${country}`;
  if (name) return name;
  return country;
}

export function isIntegerLike(value) {
  return Number.isInteger(Number(value)) && String(value) !== '';
}

export function getCookieValue(name) {
  if (typeof document === 'undefined') return '';
  const matches = document.cookie.match(new RegExp('(?:^|; )' + name + '=([^;]*)'));
  return matches ? decodeURIComponent(matches[1]) : '';
}

export function getBusinessId() {
  return getCookieValue(BUSINESS_ID_COOKIE) || sessionStorage.getItem('businessId') || '';
}

export function encodeDays(selectedDays) {
  if (!Array.isArray(selectedDays)) return 0;
  return selectedDays.reduce((acc, day) => acc | (dayBits[day] || 0), 0);
}

export function getBackendErrorMessages(value) {
  if (Array.isArray(value)) return value.map((message) => String(message)).filter(Boolean);
  if (value === null || value === undefined) return [];
  return [String(value)];
}

export function mapBackendFieldToFormField(rawField) {
  const normalized = String(rawField || '').replace(/[^a-zA-Z]/g, '').toLowerCase();
  const mapping = {
    departureairport: 'origin',
    arrivalairport: 'destination',
    scheduleddeparturetime: 'departureTime',
    scheduledarrivaltime: 'arrivalTime',
    estimatedduration: 'duration',
    frequency: 'frequency',
    aircraftid: 'aircraftId',
    touristprice: 'touristPrice',
    firstclassprice: 'firstClassPrice',
    carryonprice: 'carryOnPrice',
    checkedprice: 'checkedPrice',
  };

  return mapping[normalized] || '';
}
