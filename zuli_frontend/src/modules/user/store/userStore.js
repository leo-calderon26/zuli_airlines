import { defineStore } from "pinia";
import { ref } from "vue";

export const useUserStore = defineStore("user", () => {
    const users = ref([]);
    const pageNumber = ref(1);
    const pageSize = ref(10);
    const totalRecords = ref(0);
    const totalPages = ref(0);

    const setPaginatedData = (data, page, size, total, pages) => {
        users.value = data;
        pageNumber.value = page;
        pageSize.value = size;
        totalRecords.value = total;
        totalPages.value = pages;
    };

    return {
        users,
        pageNumber,
        pageSize,
        totalRecords,
        totalPages,
        setPaginatedData,
    };
});
