import { useReservationSearchStore } from "../store/reservationSearchStore";

export function useReservationSearch() {
    const store = useReservationSearchStore();

    const search = async (reservationCode, lastName) => {
        await store.performSearch(reservationCode, lastName);
    };

    const downloadItinerary = async (reservationCode, lastName) => {
        await store.downloadItinerary(reservationCode, lastName);
    };

    const clear = () => {
        store.clearSearch();
    };

    return {
        search,
        clear,
        downloadItinerary,
        store
    };
}