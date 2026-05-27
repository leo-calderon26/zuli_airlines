<script setup>
import { computed, ref } from 'vue';
import { useRoute } from 'vue-router';
import AdminNavBar from '../../../shared/AdminNavBar.vue';
import UserForm from '../components/UserForm.vue';
import { useUserStore } from '../store/userStore';

const route = useRoute();
const userStore = useUserStore();
const userId = computed(() => String(route.params.userId ?? ''));

const readCachedUser = () => {
    const historyUser = window.history.state?.user;

    if (historyUser?.userId === userId.value) {
        return historyUser;
    }

    const cachedUser = sessionStorage.getItem('userEditData');

    if (cachedUser) {
        try {
            const parsedUser = JSON.parse(cachedUser);

            if (parsedUser?.userId === userId.value) {
                return parsedUser;
            }
        } catch {
            sessionStorage.removeItem('userEditData');
        }
    }

    return userStore.users.find((user) => user.userId === userId.value) ?? null;
};

const selectedUser = ref(readCachedUser());
</script>

<template>
    <div class="flex min-h-svh flex-col bg-secondary">
        <AdminNavBar />
        <main class="flex-1 bg-secondary px-6 pb-32 pt-8">
            <section class="relative mx-auto flex min-h-172.5 max-w-6xl justify-center pt-8">
                <div class="absolute left-[7%] right-[5%] top-43.75 h-px bg-purple"></div>

                    <UserForm v-if="selectedUser" :user="selectedUser" :is-edit="true" />
                    <p v-else class="py-8 text-center text-sm text-content">
                        No se encontró el usuario que deseas editar. Regresa a la lista y abre Edit desde allí.
                    </p>
            </section>
        </main>
    </div>
</template>