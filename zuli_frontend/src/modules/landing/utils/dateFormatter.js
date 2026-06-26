export const arrivesNextDay = (departureStr, arrivalStr) => {
    if (!departureStr || !arrivalStr) return false;
    const dep = new Date(departureStr);
    const arr = new Date(arrivalStr);
    
    return dep.getFullYear() !== arr.getFullYear() ||
           dep.getMonth() !== arr.getMonth() ||
           dep.getDate() !== arr.getDate();
};
  
export const formatDuration = (minutes) => {
    if (!minutes) return '0h 0m';
    const h = Math.floor(minutes / 60);
    const m = minutes % 60;
    return `${h}h ${m}m`;
};
  
export const formatTime = (dateStr) => {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return d.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' });
};
  
export const getMonthShort = (dateStr) => {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return d.toLocaleDateString('es-ES', { month: 'short' }).substring(0,3).toUpperCase();
};
  
export const getDay = (dateStr) => {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return d.getDate();
};
  
export const formatDateHeader = (dateStr) => {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    const day = d.getDate();
    const month = d.toLocaleDateString('es-ES', { month: 'long' });
    return `${day} de ${month.charAt(0).toUpperCase() + month.slice(1)}`;
};
  
export const formatDateRange = (startStr) => {
    if (!startStr) return '-';
    const start = new Date(startStr);
    const startMonth = start.toLocaleDateString('es-ES', { month: 'long' });
    
    return `${start.getDate()} de ${startMonth.charAt(0).toUpperCase() + startMonth.slice(1)} `;
};

export const getRemainingTimeText = (departureDateTimeStr, arrivalDateTimeStr) => {
    if (!departureDateTimeStr || !arrivalDateTimeStr) return '';
    
    const now = new Date();
    const departureDate = new Date(departureDateTimeStr);
    const arrivalDate = new Date(arrivalDateTimeStr);
    
    if (now > arrivalDate) {
        return 'Vuelo finalizado';
    }
    
    if (now >= departureDate && now <= arrivalDate) {
        return 'Vuelo en curso';
    }
    
    const diffMs = departureDate - now;
    
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    if (diffDays > 0) {
        return `Faltan ${diffDays} días`;
    }
    
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
    if (diffHours > 0) {
        return `Faltan ${diffHours} horas`;
    }
    
    const diffMinutes = Math.floor(diffMs / (1000 * 60));
    return `Faltan ${diffMinutes} minutos`;
};