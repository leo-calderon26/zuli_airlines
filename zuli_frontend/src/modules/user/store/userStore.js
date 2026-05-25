import { defineStore } from "pinia";
import { ref } from "vue";

const createUserPayload = (user = {}) => ({
    userId: user.userId ?? "",
    personId: user.personId ?? 0,
    nationalId: user.nationalId ?? "",
    firstName: user.firstName ?? "",
    firstLastName: user.firstLastName ?? "",
    secondLastName: user.secondLastName ?? "",
    businessEmail: user.businessEmail ?? "",
    userRole: user.userRole ?? "",
    isActive: user.isActive ?? false,
});

export const useUserStore = defineStore("user", () => {
    const users = ref([]);
    const pageNumber = ref(1);
    const pageSize = ref(10);
    const totalRecords = ref(0);
    const totalPages = ref(0);

    const setPaginatedData = (data, page, size, total, pages) => {
        users.value = data.map(createUserPayload);
        pageNumber.value = page;
        pageSize.value = size;
        totalRecords.value = total;
        totalPages.value = pages;
    };

    const updateUser = (userId, userData) => {
        const index = users.value.findIndex((item) => item.userId === userId);

        if (index === -1) {
            return;
        }

        users.value[index] = createUserPayload({ ...users.value[index], ...userData, userId });
    };

    return {
        users,
        pageNumber,
        pageSize,
        totalRecords,
        totalPages,
        setPaginatedData,
        updateUser,
    };
});
