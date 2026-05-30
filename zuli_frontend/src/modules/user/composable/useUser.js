import { useUserStore } from "../store/userStore";
import userService from "../services/userService";

export function useUser() {
    const store = useUserStore();

    const fetchUsersPaginated = async (pageNumber = 1, pageSize = 10, search = "", searchType = "name") => {
        const response = await userService.getUsers({ page: pageNumber, pageSize, search, searchType });
        store.setPaginatedData(
            response.users || [],
            response.page || 1,
            response.pageSize || 10,
            response.totalItems || 0,
            response.totalPages || 1
        );
    };

    const changePage = async (pageNumber) => {
        await fetchUsersPaginated(pageNumber, store.pageSize);
    };

    return {
        fetchUsersPaginated,
        changePage,
    };
}
