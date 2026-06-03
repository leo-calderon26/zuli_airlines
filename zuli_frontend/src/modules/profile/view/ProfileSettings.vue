<script setup>
import authService from '../../auth/services/authService';

import { useRouter } from "vue-router";
const router = useRouter();

const signOut = async () => {
    try {
        await authService.logout();
    } finally {
        sessionStorage.removeItem('businessEmail');
        sessionStorage.removeItem('businessId');
        sessionStorage.removeItem('userRole');
        router.push({ name: 'reserve' });
    }
};
</script>

<template>
    <div class="py-8 px-4 mx-auto max-w-screen-2xl text-center lg:py-16">
        
        <button type="button" class="inline-flex items-center justify-center text-white bg-primary hover:bg-brand-strong box-border border border-transparent focus:ring-4 focus:ring-brand-medium shadow-xs font-medium rounded-base text-base px-5 py-3 focus:outline-none" @click="signOut">
            Cerrar Sesión
        </button>
    </div>
</template>

<style scoped>
    .landing-page {
        display: flex;
        flex-direction: column;
    }

        .landing-page > :last-child {
            margin-top: auto;
        }

    .message {
        color: #000;
    }
</style>