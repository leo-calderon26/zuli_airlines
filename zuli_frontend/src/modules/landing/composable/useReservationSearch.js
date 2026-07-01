import { useReservationSearchStore } from "../store/reservationSearchStore";

export function useReservationSearch() {
    const store = useReservationSearchStore();

    const search = async (reservationCode, lastName) => {
        await store.performSearch(reservationCode, lastName);
    };

    const clear = () => {
        store.clearSearch();
    };

    return {
        search,
        clear,
        store
    };
}