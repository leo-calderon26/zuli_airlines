export const arrivesNextDay = (departureStr, arrivalStr) => {
    if (!departureStr || !arrivalStr) return false;
    const departureDate = new Date(departureStr);
    const arrivalDate = new Date(arrivalStr);
    
    return departureDate.getFullYear() !== arrivalDate.getFullYear() ||
           departureDate.getMonth() !== arrivalDate.getMonth() ||
           departureDate.getDate() !== arrivalDate.getDate();
};
  
export const formatDuration = (totalMinutes) => {
    if (!totalMinutes) return '0h 0m';
    const hours = Math.floor(totalMinutes / 60);
    const remainingMinutes = totalMinutes % 60;
    return `${hours}h ${remainingMinutes}m`;
};
  
export const formatTime = (dateStr) => {
    if (!dateStr) return '';
    const parsedDate = new Date(dateStr);
    return parsedDate.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' });
};
  
export const getMonthShort = (dateStr) => {
    if (!dateStr) return '';
    const parsedDate = new Date(dateStr);
    return parsedDate.toLocaleDateString('es-ES', { month: 'short' }).substring(0,3).toUpperCase();
};
  
export const getDay = (dateStr) => {
    if (!dateStr) return '';
    const parsedDate = new Date(dateStr);
    return parsedDate.getDate();
};
  
export const formatDateHeader = (dateStr) => {
    if (!dateStr) return '';
    const parsedDate = new Date(dateStr);
    const day = parsedDate.getDate();
    const monthName = parsedDate.toLocaleDateString('es-ES', { month: 'long' });
    return `${day} de ${monthName.charAt(0).toUpperCase() + monthName.slice(1)}`;
};
  
export const formatDateRange = (startStr) => {
    if (!startStr) return '-';
    const startDate = new Date(startStr);
    const startMonthName = startDate.toLocaleDateString('es-ES', { month: 'long' });
    
    return `${startDate.getDate()} de ${startMonthName.charAt(0).toUpperCase() + startMonthName.slice(1)} `;
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
    
    const differenceInMilliseconds = departureDate - now;
    
    const differenceInDays = Math.floor(differenceInMilliseconds / (1000 * 60 * 60 * 24));
    if (differenceInDays > 0) {
        return `Faltan ${differenceInDays} días`;
    }
    
    const differenceInHours = Math.floor(differenceInMilliseconds / (1000 * 60 * 60));
    if (differenceInHours > 0) {
        return `Faltan ${differenceInHours} horas`;
    }
    
    const differenceInMinutes = Math.floor(differenceInMilliseconds / (1000 * 60));
    return `Faltan ${differenceInMinutes} minutos`;
};